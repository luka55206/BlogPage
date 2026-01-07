namespace BlogPage.Application.Posts;

public record CreatePostRequest(
    string Title,
    string Content,
    List<string> Tags);
  
