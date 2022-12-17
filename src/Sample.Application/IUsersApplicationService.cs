namespace Sample.Application;

public interface IUsersApplicationService
{
    Task<UserDto> GetAsync(Guid id);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUpdateUserDto input);
    Task<UserDto> UpdateAsync(Guid id, CreateUpdateUserDto input);
    Task DeleteAsync(Guid id);
}