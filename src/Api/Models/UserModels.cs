namespace Blogs.Api.Models;

public record UserModel<T>([Required] T User);

public record UserResponse(
    string Username,
    string Email,
    string Password,
    string Bio,
    string Image,
    bool IsAdmin);

public record UsersResponse(IEnumerable<UserResponse> Users, int UsersCount);
