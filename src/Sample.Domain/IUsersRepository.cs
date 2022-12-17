namespace Sample.Domain;

public interface IUsersRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task InsertAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}