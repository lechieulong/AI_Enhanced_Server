using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class ClassFile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FilePath { get; set; } 

        [Required]
        public Guid ClassId { get; set; }

        [ForeignKey("ClassId")]
        public Class Class { get; set; }

        [Required]
        public string UploadDate { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; } 
    }
}
