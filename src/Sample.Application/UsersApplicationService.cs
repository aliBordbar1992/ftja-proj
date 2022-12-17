using Sample.Ado;
using Sample.Domain;

namespace Sample.Application;

public class UsersApplicationService : IUsersApplicationService
{
    private readonly IUsersRepository _repository;

    public UsersApplicationService(IUsersRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> GetAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            return new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Credit = entity.Credit,
                Age = entity.Age
            };
        }
        catch (EntityNotFoundException e)
        {
            return null;
        }
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return (await _repository.GetAllAsync()).Select(x => new UserDto
        {
            Id = x.Id,
            Name = x.Name,
            Credit = x.Credit,
            Age = x.Age
        }).ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUpdateUserDto input)
    {
        var id = Guid.NewGuid();
        var entity = new User
        {
            Credit = input.Credit,
            Name = input.Name,
            Age = input.Age,
            CreateTime = DateTime.Now,
            Id = id
        };

        await _repository.InsertAsync(entity);
        return await GetAsync(id);
    }

    public async Task<UserDto> UpdateAsync(Guid id, CreateUpdateUserDto input)
    {
        var entity = await _repository.GetByIdAsync(id);
        entity.Credit = input.Credit;
        entity.Name = input.Name;
        entity.Age = input.Age;

        await _repository.UpdateAsync(entity);
        return await GetAsync(id);
    }

    public Task DeleteAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }
}