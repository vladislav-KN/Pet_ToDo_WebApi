using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pet_ToDo_WebApi.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; } = null!; 
        [JsonIgnore]
        public HashPasswordModel Password { get; set; } = null!;

        public ICollection<TaskModel> Tasks { get; } = new List<TaskModel>();
         
    }

   
}
