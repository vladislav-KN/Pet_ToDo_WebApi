using Pet_ToDo_WebApi.Data;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;

namespace Pet_ToDo_WebApi.Services.TaskService
{
    public interface ITaskService:IStandardServiceProvider<TaskEntity>
    {
       
        public ICollection<TaskEntity>? GetAll(string uname);
        public TaskEntity? GetById(string uname, int id);
        public Task<TaskEntity?> Update(string uname, TaskApiModel entity, int id);
        public Task<TaskEntity?> Add(string uname, TaskApiModel entity);
        public Task<bool> Delete(string uname,int id);

    }
}
