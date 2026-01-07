using System.Security.Claims;
using BlogPage.Application.Comments;
using BlogPage.Domain.Entities;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace BlogPage.Endpoints;

public static class CommentEndpoints
{
    public static IEndpointRouteBuilder MapCommentsEndpoints(this IEndpointRouteBuilder app)
    {
        // create comment
        app.MapPost("posts/{id}/comments",
            async (int id, CreateCommentRequest request, ClaimsPrincipal userPrincipal, BlogDbContext db) =>
            {
                var post = await db.Posts.FindAsync(id);
                
                if (post == null)
                    return Results.NotFound();
                
                var userIdClaim = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (userIdClaim == null)
                    return Results.Unauthorized();
                
                int authorId = int.Parse(userIdClaim);
                
                var comment = new Comment
                {
                    PostId = id,
                    UserId = authorId,
                    Content = request.Content,
                    DateCreated = DateTime.UtcNow,
                };
                
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                
                await db.Entry(comment).Reference(a => a.User).LoadAsync();
                
                return Results.Created($"comments/{comment.Id}", CommentMapper.toDto(comment));
                
                

            }
        ).RequireAuthorization();
        
        
        //delete comment by comment id
        app.MapDelete("posts/{id}/comments/{commentId}",
            async (int id, int commentId, ClaimsPrincipal userPrincipal, BlogDbContext db) =>
            {
                var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                    return Results.NotFound();
                
                if (comment.PostId != id)
                    return Results.NotFound();

                var userIdClaim = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
                    ?.Value;
                
                if (userIdClaim == null)
                    return Results.Unauthorized();
                
                int authorId = int.Parse(userIdClaim);

                if (comment.UserId != authorId)
                    return Results.Forbid();

                db.Comments.Remove(comment);

                await db.SaveChangesAsync();

                return Results.NoContent();


            }
        ).RequireAuthorization();

        
    //Get Comments by post
    app.MapGet("posts/{id}/comments", async (int id, ClaimsPrincipal userPrincipal, BlogDbContext db) =>
        {
            var post = await db.Posts.FindAsync(id);
            
            if (post == null)
                return Results.NotFound();
            
            var comments = await db.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == post.Id)
                .OrderBy(c => c.DateCreated)
                .ToListAsync();
            
            return Results.Ok(comments.Select(c => CommentMapper.toDto(c)));
            
            
        }
        
    ).RequireAuthorization();
            
        
        
    return app;
    }
}