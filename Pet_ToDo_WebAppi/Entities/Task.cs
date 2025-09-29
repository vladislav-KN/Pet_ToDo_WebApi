using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pet_ToDo_WebApi.Entities
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; }
        [JsonIgnore]
        public UserEntity Owner { get; set; } = null!;
    }
}
