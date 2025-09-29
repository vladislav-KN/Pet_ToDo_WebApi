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

        public static string GenerateSalt(int length)
        { 
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+><.,;:\"'|\\?/{}[]-=";
            Random random = new Random();
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();

        }
        public static string GetSha256Hash(SHA256 shaHash, string input)
        { 
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));
             
            StringBuilder sBuilder = new StringBuilder();
             
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
             
            return sBuilder.ToString();
        }
        public bool CheckPassword(string password)
        {
            if(User == null) return false;
            if(string.IsNullOrEmpty(password)) return false;
            if(string.IsNullOrEmpty(HashPassword)) return false;
            if(string.IsNullOrEmpty(HashSalt)) return false;
            SHA256 sHA = SHA256.Create();

            return GetSha256Hash(sHA, password + HashSalt) == HashPassword;
        }
    }

}
