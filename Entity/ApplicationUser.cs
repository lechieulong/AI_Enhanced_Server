using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? ImageURL { get; set; }
        public DateTime? DOB { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}
