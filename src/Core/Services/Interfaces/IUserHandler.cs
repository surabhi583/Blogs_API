namespace Blogs.Core.Services.Interfaces;

public interface IUserHandler
{
    public Task<UserDto> CreateAsync(NewUserDto newUser, CancellationToken cancellationToken);

    public Task<UserDto> UpdateAsync(
        string username, UpdatedUserDto updatedUser, CancellationToken cancellationToken);

    public Task<UserDto> LoginAsync(LoginUserDto login, CancellationToken cancellationToken);

    public Task<UserDto> GetAsync(string username, CancellationToken cancellationToken);

    public Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    public Task<UsersResponseDto> GetUsersAsync(UsersQuery usersQuery, CancellationToken cancellationToken);

    public Task<bool> DeleteAsync(string email, CancellationToken cancellationToken);
}
