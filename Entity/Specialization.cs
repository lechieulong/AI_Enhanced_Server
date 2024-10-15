using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Specialization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // E.g., IELTS Speaking, Writing, etc.
        public ICollection<UserEducation>? UserEducations { get; set; }
    }
}
