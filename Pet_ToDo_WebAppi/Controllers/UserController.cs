using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Models;
using System.Linq;
using System.Security.Cryptography;

namespace Pet_ToDo_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateNever]
    public class UserController: Controller
    {
        private readonly ToDoContext toDoContext = new ToDoContext();
        [HttpGet]
        public async Task<ActionResult<UserModel>> Get()
        { 
            return StatusCode(StatusCodes.Status401Unauthorized, "invalid user data");


        }

        [HttpGet("{username:required}@{password:required}")]
        public async Task<ActionResult<UserModel>> Login(string username, string password)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Include(x=>x.Password).Where(us => us.Name == username);
            if (data.Any())
            {
                foreach (var user in data)
                {
                    if (HashPasswordModel.GetSha256Hash(sHA256, password + user.Password.HashSalt) == user.Password.HashPassword)
                        return user;

                } 
            }
            return StatusCode(StatusCodes.Status401Unauthorized, "invalid user data");


        }
        [HttpPost("{username:required}@{password:required}")]
        public async Task<ActionResult<UserModel>> Register(string username, string password)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Where(us => us.Name == username);
            if (data.Any())
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "invalid user data");
            }
            var salt = HashPasswordModel.GenerateSalt(10);
            var passwordHash = HashPasswordModel.GetSha256Hash(sHA256, password + salt);
            HashPasswordModel hashPassword = new HashPasswordModel { HashPassword = passwordHash, HashSalt = salt};
            UserModel user = new UserModel { Name = username, Password = hashPassword }; 
            hashPassword.User = user;
            var resUser = await toDoContext.Users.AddAsync(user);
            var resHash = await toDoContext.HashPasswords.AddAsync(hashPassword);
            if (resUser != null && resHash != null)
            {
                int saveRes  = await toDoContext.SaveChangesAsync();
                if (saveRes > 0) 
                    return resUser.Entity;
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "cannot post database");
        }
    }
}
