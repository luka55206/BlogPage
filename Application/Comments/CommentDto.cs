namespace BlogPage.Application.Comments;

public record CommentDto
(
    string Content,
    DateTime DateCreated,
    string Author
);