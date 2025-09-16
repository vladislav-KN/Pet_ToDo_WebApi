﻿namespace Pet_ToDo_WebApi.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public int UserId { get; set; }
        public UserModel? Owner { get; set; }
    }
}
