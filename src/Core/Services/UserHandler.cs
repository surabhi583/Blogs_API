using Blogs.Infrastructure.Utils;
using Serilog;

namespace Blogs.Core.Services;

public class UserHandler(IBlogRepository repository, ITokenGenerator tokenGenerator)
    : IUserHandler
{
    public async Task<UserDto> CreateAsync(NewUserDto newUser, CancellationToken cancellationToken)
    {
        var user = new User(newUser);
        await repository.AddUserAsync(user);
        await repository.SaveChangesAsync(cancellationToken);
        var token = tokenGenerator.CreateToken(user.Username);
        return new UserDto(user.Username, user.Email, token, user.Bio, user.Image, user.IsAdmin);
    }

    public async Task<UserDto> UpdateAsync(
        string username, UpdatedUserDto updatedUser, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByUsernameAsync(username, cancellationToken);
        user.UpdateUser(updatedUser);
        await repository.SaveChangesAsync(cancellationToken);
        var token = tokenGenerator.CreateToken(user.Username);
        return new UserDto(user.Username, user.Email, token, user.Bio, user.Image, user.IsAdmin);
    }

    public async Task<UserDto> LoginAsync(LoginUserDto login, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(login.Email, cancellationToken);

        if (user == null || user.Password != login.Password)
        {
            throw new ProblemDetailsException(new ValidationProblemDetails
            {
                Status = 422,
                Detail = "Incorrect Credentials",
                Errors = { new KeyValuePair<string, string[]>("Credentials", new[] { "incorrect credentials" }) }
            });
        }

        var token = tokenGenerator.CreateToken(user.Username);
        return new UserDto(user.Username, user.Email, token, user.Bio, user.Image, user.IsAdmin);
    }

    public async Task<UserDto> GetAsync(string username, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByUsernameAsync(username, cancellationToken);
        var token = tokenGenerator.CreateToken(user.Username);
        return new UserDto(user.Username, user.Email, token, user.Bio, user.Image, user.IsAdmin);
    }

    public async Task<UsersResponseDto> GetUsersAsync(UsersQuery usersQuery, CancellationToken cancellationToken)
    {
        var usersResponseDto = await repository.GetUsersAsync(usersQuery, cancellationToken);
        return usersResponseDto;
    }

    public async Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            throw new ProblemDetailsException(new ValidationProblemDetails
            {
                Status = 422,
                Detail = "Incorrect Email",
                Errors = { new KeyValuePair<string, string[]>("Email", new[] { "incorrect email" }) }
            });
        }
        return new UserDto(user.Username, user.Email, string.Empty, user.Bio, user.Image, user.IsAdmin);
    }
    public async Task<bool> DeleteAsync(string email, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            throw new ProblemDetailsException(new ValidationProblemDetails
            {
                Status = 422,
                Detail = "User not found"
            });
        }

        repository.DeleteUser(user);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
