using System.Security.Claims;
using BlogPage.Application.Posts;
using BlogPage.Domain.Entities;
using BlogPage.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BlogPage.Endpoints;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
     //Post create post
     app.MapPost("/posts", async (CreatePostRequest request, ClaimsPrincipal userPrincipal, BlogDbContext db, PostService postService)
             =>
         {
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (userIdClaim is null)
                 return Results.Unauthorized();
             
             int authorId = int.Parse(userIdClaim);

             var postCommand = new CreatePostCommand(
                 authorId,
                 request.Title,
                 request.Content,
                 request.Tags);
             
             var postDto = await postService.CreatePostAsync(postCommand);
             
             return Results.Created($"/posts/{postDto.Id}", postDto); //{post.Id}
         }
     ).RequireAuthorization();

    //get all posts
     app.MapGet("/posts", async (BlogDbContext db) =>
         {
           

             var posts = await db.Posts
                 .Include(p=> p.Author)
                 .OrderByDescending(p => p.PublishDate)
                 .ToListAsync();
             
             var postsDto = posts.Select(PostMapper.toDto);

             return Results.Ok(postsDto);
             
             

         }
     );
     
     
     app.MapGet("posts/{id}", async (int id, BlogDbContext db) =>
     {
         
         
         var post = await db.Posts
             .Include(p => p.Author)
             .Where(post => post.Id == id)
             .FirstOrDefaultAsync();
         
         
         if(post is null)
             return Results.NotFound();
         
         return Results.Ok(PostMapper.toDto(post));

     }
     );

     app.MapPut("/posts/{id}", async (int id, UpdatePostRequest request, ClaimsPrincipal userPrincipal, BlogDbContext db) =>
         {
            
             
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             
             if(userIdClaim is null)
                 return Results.Unauthorized();
             
             var postAuthorId = int.Parse(userIdClaim);
             
             var post = await db.Posts
                              .Include(p => p.Author)
                              .Include(p => p.PostTags)
                              .FirstOrDefaultAsync(p => p.Id == id);

             if (post is null)
             {
                 return Results.NotFound();
             }
             
             if(postAuthorId != post.AuthorId)
                 return Results.Forbid();
             
             
             post.Title =  request.title; 
             post.Content = request.content;

             await db.SaveChangesAsync();
             
             return Results.Ok(PostMapper.toDto(post));

         }
         
     );


     app.MapDelete("/posts/{id}", async (int id, ClaimsPrincipal userPrincipal, BlogDbContext db) => 
         {
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             
             if (userIdClaim is null)
                 return Results.Unauthorized();
             
             var authorId = int.Parse(userIdClaim);

             var post = await db.Posts
                 .Include(p => p.Author)
                 .FirstOrDefaultAsync(p => p.Id == id);
             
             if (post is null)
                 return Results.NotFound();
             
             
             if(authorId != post.AuthorId)
                 return Results.Forbid();
             
             db.Posts.Remove(post);
             await db.SaveChangesAsync();
             
             return Results.NoContent();
             
         }
     );



     return app;
    }
}