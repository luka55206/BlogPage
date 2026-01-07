using BlogPage.Domain.Entities;

namespace BlogPage.Application.Comments;

public class CommentMapper
{
    public static CommentDto toDto(Comment comment) =>
        new CommentDto(
            comment.Content,
            comment.DateCreated,
            comment.User.Username
            );

}