using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using Pet_ToDo_WebApi.Services;
using Pet_ToDo_WebApi.Services.UserService;
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
        private readonly IUserService _standardService;
        public UserController(IConfiguration config, IUserService serviceAsync)
        {
            _config = config;
            toDoContext = new ToDoContext();
            _standardService = serviceAsync;
        }
      
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserEntity>> Login([FromBody] UsernamePasswordModel unpModel)
        {

            var user = await Authorize(unpModel.Username, unpModel.Password);
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
            var user = await _standardService.AddAsync(new UserEntity() { Name = unpModel.Username, Password = new() { HashPassword =  unpModel.Password } }); 
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
      
        private async Task<UserEntity?> Authorize(string username, string password)
        { 
            if(_standardService is UserService us)
            {
                return await us.GetAuth(username, password); 
            } 
            return null;
        }
        
    }
}
