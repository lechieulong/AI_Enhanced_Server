using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class SkillDto
    {
        public int Duration { get; set; }
        public SkillTypeEnum Type { get; set; }
        public List<PartDto> Parts { get; set; } = new List<PartDto>();

    }
}
