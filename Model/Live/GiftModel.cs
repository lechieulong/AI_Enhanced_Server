using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Live
{
    public class GiftModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}