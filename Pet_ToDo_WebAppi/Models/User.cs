namespace Pet_ToDo_WebApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int HashPasswordId { get; set; }
        public HashPasswordModel? Password { get; set; }

        public List<TaskModel> Tasks { get; set; }
    }
}
