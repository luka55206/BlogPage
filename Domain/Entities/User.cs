using System.Collections.ObjectModel;

namespace BlogPage.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }

    public string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}