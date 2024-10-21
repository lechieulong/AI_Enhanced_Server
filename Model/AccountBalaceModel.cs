using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AccountBalaceModel
    {

        public int Id { get; set; }
        public String UserId { get; set; }

        public decimal Balance { get; set; }

        public DateTime LastUpdated { get; set; }

    }
}
