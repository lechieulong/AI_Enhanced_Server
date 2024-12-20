using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Entity.Payment;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model.Payment;
using Newtonsoft.Json;

namespace TransactionValidationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOSController : ControllerBase
    {
        private readonly ITransactionRepository _transaction;
        private readonly IAccountBalanceRepository _accountBalance;
        private readonly string ChecksumKey;
        private readonly IMapper _mapper;

        public PayOSController(IMapper mapper, IConfiguration configuration, IAccountBalanceRepository accountBalanceRepository, ITransactionRepository transactionRepository)
        {
            _transaction = transactionRepository;
            _accountBalance = accountBalanceRepository;
            _mapper = mapper;
            ChecksumKey = configuration["ApiSettings:ChecksumKey"];
        }
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateTransaction([FromBody] ApiResponse request)

        {
            var transactionData = request.Data;
            if (IsValidData(request.Data, request.Signature, ChecksumKey))
            {
                if (transactionData.Code.Equals("00"))
                {
                 var oder = await _transaction.GetTransactionByIdAsync(transactionData.OrderCode);
                    oder.PaymentStatus = "PAID";
                    await _transaction.UpdateTransactionAsync(_mapper.Map<TransactionModel>(oder));
                    var ba = new AccountBalaceModel
                    {
                        Balance = transactionData.Amount,
                        Message = "deposit money into account",
                        signature = "",
                        Type = "deposit",
                        UserId = oder.UserId,
                    };
                    _accountBalance.UpdateBalance(ba);
                    return Ok(new { IsValid = true });
                }
                else
                {
                    var oder = await _transaction.GetTransactionByIdAsync(transactionData.OrderCode);
                    oder.PaymentStatus = "CANCELLED";
                    await _transaction.UpdateTransactionAsync(_mapper.Map<TransactionModel>(oder));

                }
                
                return Ok(new { IsValid = true });
            }
            else
            {
                return BadRequest(new { IsValid = false });
            }
        }

        private static bool IsValidData(TransactionData data, string transactionSignature,String ChecksumKey)
        {
            try
            {
                var jsonObject = JsonConvert.SerializeObject(data);
                var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject);
                var sortedKeys = jsonData.Keys.OrderBy(k => k).ToList();

                var transactionStr = new StringBuilder();
                for (int i = 0; i < sortedKeys.Count; i++)
                {
                    var key = sortedKeys[i];
                    var value = jsonData[key].ToString();
                    transactionStr.Append(key);
                    transactionStr.Append('=');
                    transactionStr.Append(value!=null?value:"");

                    if (i < sortedKeys.Count - 1)
                    {
                        transactionStr.Append('&');
                    }
                }

                var signature = ComputeHmacSha256(transactionStr.ToString(), ChecksumKey);
                return signature.Equals(transactionSignature, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                // Chuyển đổi dữ liệu vào byte[]
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Chuyển đổi mảng byte thành chuỗi hex
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    public class ApiResponse
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public TransactionData Data { get; set; }
        public string Signature { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("orderCode")]
        public int OrderCode { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("transactionDateTime")]
        public string TransactionDateTime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("paymentLinkId")]
        public string PaymentLinkId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("counterAccountBankId")]
        public string CounterAccountBankId { get; set; }

        [JsonProperty("counterAccountBankName")]
        public string CounterAccountBankName { get; set; }

        [JsonProperty("counterAccountName")]
        public string CounterAccountName { get; set; }

        [JsonProperty("counterAccountNumber")]
        public string CounterAccountNumber { get; set; }

        [JsonProperty("virtualAccountName")]
        public string VirtualAccountName { get; set; }

        [JsonProperty("virtualAccountNumber")]
        public string VirtualAccountNumber { get; set; }
    }
}
