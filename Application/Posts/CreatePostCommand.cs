namespace BlogPage.Application.Posts;

public record CreatePostCommand(    
    int AuthorId,
    string Title,
    string Content,
    List<string> Tags);