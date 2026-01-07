namespace BlogPage.Domain.Entities;

public class  Post
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public DateTime PublishDate { get; set; }

    public ICollection<PostTags> PostTags { get; set; } = new List<PostTags>();
    
    public User Author { get; set; }
    
    public int AuthorId { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}