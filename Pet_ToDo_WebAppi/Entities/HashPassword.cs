using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Pet_ToDo_WebApi.Entities
{
    public class HashPasswordEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string HashPassword { get; set; } = null!;
        [Required]
        public string HashSalt { get; set; } = null!;
        [Required]
        [JsonIgnore]
        public UserEntity User { get; set; } = null!;
 
    }

}
