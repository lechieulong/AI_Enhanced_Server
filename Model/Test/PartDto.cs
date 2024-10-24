using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class PartDto
    {
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public string? Audio { get; set; }
        public List<SectionDto> Sections { get; set; }
    }
}
