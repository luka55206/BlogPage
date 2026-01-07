using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogPage.Application.Users;
using BlogPage.Domain.Entities;
using BlogPage.Persistence.Context;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogPage.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        // POST /users/register
        app.MapPost("/users/register", async (RegisterUserRequest request, BlogDbContext  db) =>
            {
                var exists = await db.Users.AnyAsync( u => u.Email == request.Email || u.Username == request.Username);

                if (exists)
                {
                    return Results.BadRequest("User Already Exists");
                }

                var user = new User();
                
                user.Email = request.Email;
                user.Username = request.Username;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                
                
                db.Users.Add(user);
                await db.SaveChangesAsync();
                
                return Results.Created($"/users/{user.Id}", UserMapper.ToDto(user));
                
                //return Results.Ok("User Created");

            }

    
        );

        app.MapPost("users/login", async (LoginRequest request, BlogDbContext db) =>
        {
            
            
            var user = await db.Users.FirstOrDefaultAsync( u => u.Email == request.Email);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            if (request.Email != user.Email || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.UTF8.GetBytes("adzinoki_volki_matraxs_ar_dairtyams");

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256);
            
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = "BlogPage",
                Audience = "BlogPageUsers",
                SigningCredentials = credentials
            }; 

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);


            return Results.Ok(new { token = jwt });

        });


        app.MapGet("/users/me", async (ClaimsPrincipal userPrincipal, BlogDbContext db) =>
        {
             var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

             if (string.IsNullOrEmpty(userIdClaim))
             {
                 return Results.Unauthorized();
             }
             
             if (!int.TryParse(userIdClaim, NumberStyles.Integer, CultureInfo.InvariantCulture, out int userIdInt))
             {
                 // This handles cases where the claim value is present but not a valid integer.
                 // This is a server configuration/token issue, not a user error.
                 return Results.Problem("User identifier in token is not a valid number.", statusCode: StatusCodes.Status500InternalServerError);
             }
             
             var user = await db.Users.FirstOrDefaultAsync( u => u.Id == userIdInt);
             if (user == null)
             {
                 return Results.NotFound();
             }

             return Results.Ok(UserMapper.ToDto(user));
             
             
        }).RequireAuthorization();
        return app;
    }

}