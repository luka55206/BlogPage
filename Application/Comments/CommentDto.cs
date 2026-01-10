namespace BlogPage.Application.Comments;

public record CommentDto
(   int    Id,
    string Content,
    DateTime DateCreated,
    string Author
);