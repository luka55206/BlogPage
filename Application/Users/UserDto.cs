namespace BlogPage.Application.Users;

public record UserDto(  
    int id,
    string Username,
    string Email,
    DateTime CreatedAt,
    string Role);