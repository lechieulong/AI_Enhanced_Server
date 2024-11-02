using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class Gift
    {
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<User_Gift>? User_Gifts { get;set; }
    }
}
