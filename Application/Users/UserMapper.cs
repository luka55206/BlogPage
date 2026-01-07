using BlogPage.Domain.Entities;

namespace BlogPage.Application.Users;

public static class UserMapper
{
    public static UserDto ToDto(this User user) =>
        new UserDto(user.Id,
            user.Username,
            user.Email,
            user.CreatedAt,
            user.Role);
}