namespace BlogPage.Application.Comments;

public record CreateCommentCommand(
    int PostId,
    int UserId,
    string Content
    );