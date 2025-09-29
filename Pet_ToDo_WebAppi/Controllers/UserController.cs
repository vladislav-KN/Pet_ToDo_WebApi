using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pet_ToDo_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class UserController: Controller
    {
        private readonly ToDoContext toDoContext;
        private readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
            toDoContext = new ToDoContext();
        }
      
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserEntity>> Login([FromBody] UsernamePasswordModel unpModel)
        {

            var user = Authorize(unpModel.Username, unpModel.Password);
            if (user != null)
            {
                var token = GenerateToken(user);
                return StatusCode(StatusCodes.Status200OK, token);
            } 
            return StatusCode(StatusCodes.Status404NotFound, "user not found"); 
        }

      
        [HttpPost("register")]
        public async Task<ActionResult<UserEntity>> Register([FromBody] UsernamePasswordModel unpModel)
        {
            var user = await Register(unpModel.Username, unpModel.Password);
            if(user != null)
            {
                var token = GenerateToken(user);
                if(token!= null)
                { 
                    int saveRes = await toDoContext.SaveChangesAsync();
                    if (saveRes > 0)
                        return StatusCode(StatusCodes.Status200OK, token);
                }
                else
                {
                    var resUser = toDoContext.Users.Remove(user);
                    var resHash = toDoContext.HashPasswords.Remove(user.Password);
                }
              
            }
            return StatusCode(StatusCodes.Status406NotAcceptable, "cannot post database");
        }
        private string GenerateToken(UserEntity user)
        { 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Api:IssuerSigningKey"] ??"123"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name)
            };
            var token = new JwtSecurityToken(
                _config["Api:Issuer"], 
                _config["Api:Audience"], 
                claims, 
                expires: DateTime.Now.AddMinutes(1), 
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserEntity?> Register(string username, string password)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Where(us => us.Name == username);
            if (data.Any())
            {
                return null;
            }
            var salt = HashPasswordEntity.GenerateSalt(10);
            var passwordHash = HashPasswordEntity.GetSha256Hash(sHA256, password + salt);
            HashPasswordEntity hashPassword = new HashPasswordEntity { HashPassword = passwordHash, HashSalt = salt };
            UserEntity user = new UserEntity { Name = username, Password = hashPassword };
            hashPassword.User = user;
            var resUser = await toDoContext.Users.AddAsync(user);
            var resHash = await toDoContext.HashPasswords.AddAsync(hashPassword);
            if (resUser != null && resHash != null)
            {
                return resUser.Entity; 
            }
            return null;
        }

      
        private UserEntity? Authorize(string username, string password)
        {
            SHA256 sHA256 = SHA256.Create();
            var data = toDoContext.Users.Include(x => x.Password).Where(us => us.Name ==  username);
            if (data.Any())
            {
                foreach (var user in data)
                {
                    if (HashPasswordEntity.GetSha256Hash(sHA256, password + user.Password.HashSalt) == user.Password.HashPassword)
                        return user;

                }
            }
            return null;
        }
        
    }
}
