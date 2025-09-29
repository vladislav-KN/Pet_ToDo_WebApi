namespace Pet_ToDo_WebApi.Models
{
    public class TaskApiModel
    {
        public string? Name { get; set; } 

        public string? Description { get; set; }  
        public bool IsCompleted { get; set; }
    }
}
