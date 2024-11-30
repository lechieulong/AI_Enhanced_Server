using System;

namespace Model
{
    public class ClassFileDto
    {
        public string FilePath { get; set; } // Path to the file in cloud storage
        public Guid ClassId { get; set; } 
        public DateTime UploadDate { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
    }
}
