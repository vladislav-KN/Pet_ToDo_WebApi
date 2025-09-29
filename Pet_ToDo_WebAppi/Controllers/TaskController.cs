using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using System.Security.Claims;

namespace Pet_ToDo_WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ToDoContext toDoContext = new ToDoContext();
        [HttpGet]
        public async Task<ActionResult<ICollection<TaskEntity>>> GetTasks()
        {
            UserEntity? user = FindUser();
            if (user != null)
            {
                return Ok(user.Tasks);
            }
            return NotFound( new List<TaskEntity>());
        }
        [HttpGet("byid")]
        public async Task<ActionResult<TaskEntity>> GetTask(int id)
        { 
            UserEntity? user = FindUser();
            if (user != null)
            {
                TaskEntity taskEntity = user.Tasks.Where(task => task.Id==id).First();
                if (taskEntity!=null) 
                    return Ok(taskEntity);
            }
           
            return NotFound(null);
        }
        [HttpPost]
        public async Task<ActionResult<TaskEntity>> SaveData([FromBody] TaskApiModel task)
        {
            UserEntity? user = FindUser();
           
            if (user != null)
            {
                TaskEntity newTask = new TaskEntity()
                {
                    Name = task.Name ?? "",
                    Description = task.Description ?? "",
                    IsCompleted = task.IsCompleted,
                    Owner = user
                };
                toDoContext.Tasks.Add(newTask);
                user.Tasks.Add(newTask);
                toDoContext.Users.Update(user);
                await toDoContext.SaveChangesAsync();
                return Ok(newTask);
            }
            
            return NotFound(null);
        }
        [HttpPut]
        public async Task<ActionResult<TaskEntity>> SaveData([FromBody] TaskApiModel task, int id)
        {
            UserEntity? user = FindUser();

            if (user != null)
            {
                var taskUpdate = toDoContext.Tasks.Where(task => task.Id == id && task.Owner.Id == user.Id).First();
                taskUpdate.Name = task.Name!;
                taskUpdate.Description = task.Description!;
                taskUpdate.IsCompleted = task.IsCompleted;

                toDoContext.Tasks.Update(taskUpdate);
                toDoContext.Users.Update(user);
                await toDoContext.SaveChangesAsync();

                return Ok(taskUpdate);
            }

            return NotFound(null);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteData([FromBody] int id)
        {
            UserEntity? user = FindUser();

            if (user != null)
            {  
                await toDoContext.Tasks.Where(task => task.Id == id && task.Owner.Id == user.Id).ExecuteDeleteAsync();

                return Ok();
            }

            return NotFound();
        }
        private UserEntity? FindUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                string? name = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                UserEntity user = toDoContext.Users.Include(x=>x.Tasks).First(x => x.Name == name);
                if (user != null)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
