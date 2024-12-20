using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class SkillResult
    {
        public Guid Id { get; set; }  // This should be a Guid
        public Guid SkillId { get; set; }  // This should be a Guid
        public List<int> TotalParts { get; set; }  // This should be a list of integers
    }

    public class ResultPayloadDto
    {
        public List<SkillResult> SkillResultIds { get; set; }  // List of SkillResult objects
    }
}
