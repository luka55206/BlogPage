using BlogPage.Domain.Entities;
using BlogPage.Domain.Exceptions;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BlogPage.Application.Comments;

public class CommentService
{
    private readonly BlogDbContext _db;

    public CommentService(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentCommand commentCommand)
    {
        var post = await _db.Posts.FindAsync(commentCommand.PostId);
                
        if (post == null)
            throw new NotFoundException($"Post with id {commentCommand.PostId} does not exist");
                
                
        var comment = new Comment
        {
            PostId = commentCommand.PostId,
            UserId = commentCommand.UserId,
            Content = commentCommand.Content,
            DateCreated = DateTime.UtcNow,
        };
                
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
                
        await _db.Entry(comment).Reference(a => a.User).LoadAsync();
                
        return CommentMapper.toDto(comment);
    }
}