namespace Blogs.Core.Dto;

public record NewUserDto(string Username, string Email, string Password, bool IsAdmin = false);

public record LoginUserDto(string Email, string Password);

public record UpdatedUserDto(string? Username, string? Email, string? Bio, string? Image, string? Password, bool? IsAdmin);

public record UserDto(string Username, string Email, string Token, string Bio, string Image, bool IsAdmin);

public record UsersQuery(int Limit = 20, int Offset = 0);

public record UsersResponseDto(List<User> Users, int UsersCount);
