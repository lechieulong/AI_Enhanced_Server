using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public string Link { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
