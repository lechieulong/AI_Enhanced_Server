using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;
using Service;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Model;
using Microsoft.IdentityModel.Tokens;
using IService;
using Entity.Payment;
using Model.Payment;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOSController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transaction;
        private readonly IAccountBalanceRepository _accountBalance;

        public PayOSController(IMapper mapper, ITransactionRepository transaction, IAccountBalanceRepository accountBalance)
        {
            _mapper = mapper;
            _transaction = transaction;
            _accountBalance = accountBalance;
        }
        [HttpGet("PaymentResult")]
        public async Task<IActionResult> HandlePaymentResult(string code, string id, string cancel, string status, int orderCode)
        {

            try
            {
                if (code == "00" && status == "CANCELLED" && cancel == "true")
                {
                    // Payment was cancelled
                    var order = await _transaction.GetTransactionByIdAsync(orderCode);
                    if (order != null)
                    {
                        order.PaymentStatus = "CANCELLED";

                        await _transaction.UpdateTransactionAsync(_mapper.Map<TransactionModel>(order));
                    }
                    return Redirect($"https://aiilprep.azurewebsites.net/Paymentresult?code={code}&id={id}&cancel={cancel}&status={status}&orderCode={orderCode}");
                }
                else if (code == "00" && status == "PAID")
                {
                    // Payment was successful
                    var order = await _transaction.GetTransactionByIdAsync(orderCode);
                    if (order != null)
                    {
                        order.PaymentStatus = "PAID";
                        await _transaction.UpdateTransactionAsync(_mapper.Map<TransactionModel>(order));

                        // Deposit money into user's account
                        var ba = new AccountBalaceModel
                        {
                            UserId = order.UserId,
                            Balance = order.Amount,
                            Message = "deposit money to account",
                            signature = "",
                            Type = "deposit"
                        };
                        await _accountBalance.UpdateBalance(ba);
                    }
                    return Redirect($"https://aiilprep.azurewebsites.net/Paymentresult?code={code}&id={id}&cancel={cancel}&status={status}&orderCode={orderCode}");
                }
                else
                {
                    // Payment status unknown
                    return BadRequest("Payment status unknown.");
                }
            }
            catch (Exception ex)
            {
                // Log error and return server error response
                Console.Error.WriteLine($"Error handling payment result: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the payment result.");
            }


        }
    }
}