using Pet_ToDo_WebApi.Entities;

namespace Pet_ToDo_WebApi.Services.UserService
{
    public interface IUserService:IStandardServiceProvider<UserService>
    {
        public Task<UserEntity?> GetAuth(string username, string password);
        public Task<UserEntity?> AddAsync(UserEntity user);
    }
}
