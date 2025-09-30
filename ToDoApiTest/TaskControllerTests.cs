using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pet_ToDo_WebApi.Controllers;
using Pet_ToDo_WebApi.Entities;
using Pet_ToDo_WebApi.Models;
using Pet_ToDo_WebApi.Services.TaskService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApiTest
{
    public class TaskControllerTests
    {
        public static IEnumerable<object[]> TestData =>
                      new List<object[]>
                      {
                            new object[] { "string" },
                            new object[] { "string",1 },
                            new object[] { "string",3 },
                            new object[] { "string",6 },
                            new object[] { "string",4 },
                            new object[] { "string", new TaskApiModel() { Name="Aboba", Description="Bibia", IsCompleted=true } },
                            new object[] { "string", new TaskApiModel() { Name="Aboba", Description="Bibia", IsCompleted=true }, 1},
                            new object[] { "string",  1},
                            new object[] { "string",  123},
                      };

        private readonly Mock<ITaskService> taskService;
        public TaskControllerTests()
        {
            taskService = new Mock<ITaskService>();
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: [0,1])]
        public async Task GetTaskCollection_TaskEntityCollection(string user)
        {  
            //arrange
            var taskList = GetTasksData();
            taskService.Setup(x => x.GetAll(user))
                .Returns(taskList);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier,user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.GetTasks());


            //assert

            Assert.NotNull(taskResult);
            Assert.Equal(typeof(OkObjectResult), taskResult.Result?.GetType());
            var result = (taskResult.Result as OkObjectResult)?.Value as ICollection<TaskEntity>;
            Assert.NotNull(result);  
            Assert.Equal(taskList.Count(), result.Count());
            Assert.Equal(taskList.ToString(), result.ToString());
            Assert.True(taskList.Equals(result));

        }

        [Theory]
        [MemberData(nameof(GetData), parameters: [1, 4])]
        public async Task GetTaskByIndex_TaskEntity(string user, int index)
        {
            //arrange
            var task= GetTasksData().Where(x => x.Id == index).First();
            taskService.Setup(x => x.GetById(user, index))
                .Returns(task);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.GetTask(index));


            //assert

            Assert.NotNull(taskResult);
            Assert.Equal(typeof(OkObjectResult), taskResult.Result?.GetType());
            var result = (taskResult.Result as OkObjectResult)?.Value as  TaskEntity;
            Assert.NotNull(result); 
            Assert.True(task.Equals(result)); 
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: [5, 1])]
        public async Task AddTask_TaskEntity(string user, TaskApiModel taskApiModel)
        {
            //arrange
            TaskEntity taskEntity = new TaskEntity()
            {
                Id = 8,
                IsCompleted = taskApiModel.IsCompleted,
                Description = taskApiModel.Description!,
                Name = taskApiModel.Name!,
            };
            taskService.Setup(x => x.Add(user, taskApiModel))
                .ReturnsAsync(taskEntity);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.AddTask(taskApiModel));


            //assert

            Assert.NotNull(taskResult);
            Assert.Equal(typeof(OkObjectResult), taskResult.Result?.GetType());
            var result = ((taskResult.Result as OkObjectResult)?.Value as TaskEntity) ;
            Assert.NotNull(result);
            Assert.True(taskEntity.Equals(result));
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: [6, 1])]
        public async Task UpdateTask_TaskEntity(string user, TaskApiModel taskApiModel, int id)
        {
            //arrange
            TaskEntity taskEntity = new TaskEntity()
            {
                Id = 1,
                IsCompleted = taskApiModel.IsCompleted,
                Description = taskApiModel.Description!,
                Name = taskApiModel.Name!,
            };
            taskService.Setup(x => x.Update(user, taskApiModel, id))
                .ReturnsAsync(taskEntity);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.UpdateTask(taskApiModel,id));


            //assert 
            Assert.NotNull(taskResult);
            Assert.Equal(typeof(OkObjectResult), taskResult.Result?.GetType());
            var result = ((taskResult.Result as OkObjectResult)?.Value as TaskEntity);
            Assert.NotNull(result);
            Assert.True(taskEntity.Equals(result));
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: [7, 1])]
        public async Task DeleteTask_True(string user,  int id)
        {
            //arrange
            
            taskService.Setup(x => x.Delete(user,  id))
                .ReturnsAsync(true);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.DeleteData(id));


            //assert 
            Assert.NotNull(taskResult);
            Assert.Equal( typeof(OkResult), taskResult.GetType()); 
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: [8, 1])]
        public async Task DeleteTask_False(string user, int id)
        {
            //arrange

            taskService.Setup(x => x.Delete(user, id))
                .ReturnsAsync(false);
            var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, user)]);
            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var taskController = new TaskController(taskService.Object)
            {
                ControllerContext = controllerContext,
            };

            //act
            var taskResult = (await taskController.DeleteData(id)); 
            //assert 
            Assert.NotNull(taskResult);
            Assert.Equal(typeof(NotFoundResult),taskResult.GetType());
        }
        private ICollection<TaskEntity> GetTasksData()
        {
            var tasks = new List<TaskEntity>
            {
                new TaskEntity
                {
                    Id = 1,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                },
                new TaskEntity
                {
                    Id = 2,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                },
                new TaskEntity
                {
                    Id = 3,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                },
                new TaskEntity
                {
                    Id = 4,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                },
                new TaskEntity
                {
                    Id = 6,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                },
                new TaskEntity
                {
                    Id = 7,
                    Name = "string",
                    Description = "string",
                    IsCompleted = true
                }
            };
            return tasks;
        }

        public static IEnumerable<object[]> GetData(int noOfElementToSkip, int noOfElementsToTake)
        { 
            return TestData.Skip(noOfElementToSkip).Take(noOfElementsToTake);
        }

       

    }
}
