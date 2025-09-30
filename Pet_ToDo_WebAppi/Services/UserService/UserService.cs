using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pet_ToDo_WebApi.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ToDoContext toDoContext;

        public UserService(ToDoContext dbContext)
        {
            toDoContext = dbContext;
        }

        public async Task<UserEntity?> GetAuth(string username, string password)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Include(x => x.Password).Where(us => us.Name == username);
            if (data.Any())
            {
                foreach (var user in data)
                {
                    if (GetSha256Hash(sHA256, password + user.Password.HashSalt) == user.Password.HashPassword)
                        return user;
                    await Task.Delay(1);
                }
            }
            return null;
        }

        public async Task<UserEntity?> AddAsync(UserEntity user)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Where(us => us.Name == user.Name);
            if (data.Any())
            {
                return null;
            }
            var salt = GenerateSalt(10);
            var passwordHash = GetSha256Hash(sHA256, user.Password.HashPassword + salt);
            HashPasswordEntity hashPassword = new HashPasswordEntity { HashPassword = passwordHash, HashSalt = salt };
            user.Password = hashPassword;
            hashPassword.User = user;
            var resUser = await toDoContext.Users.AddAsync(user);
            var resHash = await toDoContext.HashPasswords.AddAsync(hashPassword);
            if (resUser != null && resHash != null)
            {
                return resUser.Entity;
            }
            return null;
        }
 

      
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

      
    }
}
