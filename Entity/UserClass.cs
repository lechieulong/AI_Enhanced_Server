using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class UserClass
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ClassId { get; set; }

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
