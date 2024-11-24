using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Test
{
    public class AttempTest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public int TotalAttempt {  get; set; }
    }
}
