using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model
{
    public class UserClassDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public Guid ClassId { get; set; }

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<ClassDto>? Classes { get; set; }

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<UserDto>? Users { get; set; }
    }
}
