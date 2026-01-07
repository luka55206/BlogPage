namespace BlogPage.Application.Posts;

public record PostDto
(
    int Id,
    String Title,
    String Content,
    DateTime PublishDate,
    String Author,
    List<string> Tags
);