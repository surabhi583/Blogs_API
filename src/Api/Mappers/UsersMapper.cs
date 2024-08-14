namespace Blogs.Api.Mappers
{
    public static class UsersMapper
    {
        public static UserResponse MapFromUserEntity(User user)
        {
            var result = new UserResponse(user.Username, user.Email, user.Password, user.Bio, user.Image, user.IsAdmin);
            return result;
        }

        public static UsersResponse MapFromUsers(UsersResponseDto usersResponseDto)
        {
            var users = usersResponseDto.Users
                .Select(userEntity => MapFromUserEntity(userEntity))
                .ToList();
            return new UsersResponse(users, usersResponseDto.UsersCount);
        }
    }
}
