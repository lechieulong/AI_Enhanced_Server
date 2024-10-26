using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Payment
{
    public class AccountBalaceModel
    {
        public string UserId { get; set; }

        public decimal Balance { get; set; }

        public string Message { get; set; }
        public string signature { get; set; }

    }
}
