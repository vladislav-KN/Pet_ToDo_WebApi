using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pet_ToDo_WebApi.Entities
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; } = null!; 
        [JsonIgnore]
        public HashPasswordEntity Password { get; set; } = null!;

        public ICollection<TaskEntity> Tasks { get; } = new List<TaskEntity>();
         
    }

   
}
