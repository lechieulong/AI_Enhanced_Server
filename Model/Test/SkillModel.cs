using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class SkillModel
    {
        public Guid Id { get; set; }
        public SkillTypeEnum SKillType { get; set; }
        public int Duration { get; set; }
        public List<PartModel> Parts { get; set; } = new List<PartModel>();

    }
}
