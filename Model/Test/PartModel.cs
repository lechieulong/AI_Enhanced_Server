using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class PartModel
    {
        public Guid Id { get; set; }
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public string Image { get; set; }
        public string Audio { get; set; }
        public List<SectionModel> Section { get; set; } = new List<SectionModel>();
    }
}
