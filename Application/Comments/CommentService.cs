using BlogPage.Domain.Entities;
using BlogPage.Domain.Exceptions;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BlogPage.Application.Comments;

public class CommentService
{
    private readonly BlogDbContext _db;
    private readonly ILogger<CommentService> _logger;

    public CommentService(BlogDbContext db, ILogger<CommentService> logger)
    {
        _db = db;   
        _logger = logger;
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentCommand commentCommand)
    {
        
            _logger.LogInformation(
                "Creating comment on post {PostId} by user {UserId}", 
                commentCommand.PostId, 
                commentCommand.UserId);
            
            var post = await _db.Posts.FindAsync(commentCommand.PostId);

            if (post == null)
            {
                _logger.LogWarning(
                    "Attempt to comment on non-existent post {PostId}", 
                    commentCommand.PostId);
                throw new NotFoundException($"Post with id {commentCommand.PostId} does not exist");

            }


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
            
            _logger.LogInformation(
                "Comment {CommentId} created successfully", 
                comment.Id);
            
            return CommentMapper.toDto(comment);
        
        
    }
}