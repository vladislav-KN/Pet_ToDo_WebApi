using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pet_ToDo_WebApi.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly ToDoContext toDoContext;
        public TaskService(ToDoContext dbContext)
        {
            toDoContext = dbContext;
        }

        public async Task<TaskEntity?> Add(string uname, TaskApiModel task)
        {
            UserEntity? user = GetUser(uname);

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
                return newTask;
            }
            return null;
        }

        public async Task<bool> Delete(string uname, int id)
        {
            UserEntity? user = GetUser(uname);

            if (user != null)
            {
                await toDoContext.Tasks.Where(task => task.Id == id && task.Owner.Id == user.Id).ExecuteDeleteAsync();
                return true;
            }
            return false;
        }

        public ICollection<TaskEntity>? GetAll(string uname)
        {
            return GetUser(uname)?.Tasks;
        }

        public TaskEntity? GetById(string uname, int id)
        {
            UserEntity? user = GetUser(uname);
            if (user != null)
            {
                TaskEntity taskEntity = user.Tasks.Where(task => task.Id == id).First();
                if (taskEntity != null)
                    return taskEntity;
            }
            return null;
        }

        public UserEntity? GetUser(string name)
        {
            return toDoContext.Users.Include(x => x.Tasks).First(x => x.Name == name); 
        }

        public async Task<TaskEntity?> Update(string uname, TaskApiModel task, int id)
        {
            UserEntity? user = GetUser(uname);

            if (user != null)
            {
                var taskUpdate = toDoContext.Tasks.Where(task => task.Id == id && task.Owner.Id == user.Id).First();
                taskUpdate.Name = task.Name!;
                taskUpdate.Description = task.Description!;
                taskUpdate.IsCompleted = task.IsCompleted;

                toDoContext.Tasks.Update(taskUpdate);
                toDoContext.Users.Update(user);
                await toDoContext.SaveChangesAsync();

                return taskUpdate;
            }
            return null;
        }
    }
}
