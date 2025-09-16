using System.ComponentModel.DataAnnotations;

namespace Pet_ToDo_WebApi.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; } 
        public UserModel Owner { get; set; } = null!;
    }
}
