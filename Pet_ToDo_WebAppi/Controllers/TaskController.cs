using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using Pet_ToDo_WebApi.Services;
using Pet_ToDo_WebApi.Services.TaskService;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pet_ToDo_WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    { 
        private readonly ITaskService _dbService;
        public TaskController(ITaskService service)
        {
            _dbService=service;
        }
        [HttpGet]
        public async Task<ActionResult<ICollection<TaskEntity>>> GetTasks()
        {
            var res = _dbService.GetAll(FindUser()??"");
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound();
        }
        [HttpGet("byid")]
        public async Task<ActionResult<TaskEntity>> GetTask(int id)
        {
            var res = _dbService.GetById(FindUser() ?? "",id);
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound(); 
        }
        [HttpPost]
        public async Task<ActionResult<TaskEntity>> AddTask([FromBody] TaskApiModel task)
        {
            var res = await _dbService.Add(FindUser() ?? "", task);
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound();
        }
        [HttpPut]
        public async Task<ActionResult<TaskEntity>> UpdateTask([FromBody] TaskApiModel task, int id)
        {
            var res = await _dbService.Update(FindUser() ?? "", task,id);
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound();
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteData([FromBody] int id)
        {
            var res = await _dbService.Delete(FindUser() ?? "",  id);
            if (res == true)
            {
                return Ok();
            }
            return NotFound();
        }
        private string? FindUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
               
            }
            return null;
        }
    }
}
