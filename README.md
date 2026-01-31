BlogPage API
A production-ready RESTful API for a blogging platform built with .NET 9, featuring JWT authentication, comprehensive validation, pagination, and testing.



🚀 Features
✅ JWT Authentication - Secure token-based authentication
✅ CRUD Operations - Posts, Comments, Users
✅ Tag System - Many-to-many relationships
✅ Input Validation - FluentValidation for all endpoints
✅ Pagination & Filtering - Efficient data loading
✅ Search - Find posts by title, content, tags, author
✅ Global Error Handling - Consistent error responses
✅ Structured Logging - Serilog with file and console output
✅ CORS Support - Frontend integration ready
✅ Health Checks - Monitor API status
✅ Unit & Integration Tests - Comprehensive test coverage
✅ Swagger Documentation - Interactive API docs
🛠️ Tech Stack
.NET 9 - Latest .NET framework
ASP.NET Core - Minimal APIs
Entity Framework Core 8 - ORM with Code-First approach
SQLite - Lightweight database
JWT Bearer - Authentication & Authorization
BCrypt - Password hashing
FluentValidation - Input validation
Serilog - Structured logging
xUnit - Unit testing
FluentAssertions - Test assertions
Swagger/OpenAPI - API documentation
📋 Prerequisites
.NET 9 SDK
IDE (Visual Studio, VS Code, or Rider)
🚀 Getting Started
1. Clone the repository
bash
git clone https://github.com/luka55206/BlogPage.git
cd BlogPage
2. Run migrations
bash
cd BlogPage
dotnet ef database update
3. Run the application
bash
dotnet run
The API will be available at:

Swagger: http://localhost:5102/swagger
Health Check: http://localhost:5102/health
📚 API Documentation
Authentication
Register
http
POST /users/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "Password123!"
}
Login
http
POST /users/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123!"
}

Response: { "token": "eyJhbGciOiJIUzI1NiIs..." }
Posts
Get all posts (with pagination & filtering)
http
GET /posts?page=1&pageSize=20
GET /posts?search=react&tag=tech
GET /posts?sortBy=title&sortOrder=asc
Create post
http
POST /posts
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "My First Post",
  "content": "This is the content...",
  "tags": ["tech", "dotnet"]
}
Update post
http
PUT /posts/{id}
Authorization: Bearer {token}

{
  "title": "Updated Title",
  "content": "Updated content..."
}
Delete post
http
DELETE /posts/{id}
Authorization: Bearer {token}
Comments
Get comments
http
GET /posts/{id}/comments?page=1&pageSize=20
Create comment
http
POST /posts/{id}/comments
Authorization: Bearer {token}

{
  "content": "Great post!"
}
🧪 Running Tests
bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific tests
dotnet test --filter "FullyQualifiedName~PostServiceTests"
📁 Project Structure
BlogPage/
├── Domain/                  # Entities and exceptions
│   ├── Entities/
│   └── Exceptions/
├── Application/             # Business logic and DTOs
│   ├── Posts/
│   ├── Comments/
│   ├── Users/
│   └── Common/
├── Persistence/             # Database context
│   └── Context/
├── Endpoints/               # API endpoints
├── Middleware/              # Custom middleware
└── Program.cs               # App configuration

BlogPage.Tests/
├── Services/                # Unit tests
└── Integration/             # Integration tests
🔒 Security Features
✅ Passwords hashed with BCrypt
✅ JWT tokens with expiration
✅ Bearer token authorization
✅ Input validation on all endpoints
✅ SQL injection prevention (EF Core)
✅ Global exception handling
✅ CORS configuration
📊 Performance Features
✅ Pagination on list endpoints
✅ Efficient database queries
✅ Async/await throughout
✅ In-memory caching ready
✅ Health checks for monitoring
🤝 Contributing
Fork the repository
Create feature branch (git checkout -b feature/amazing-feature)
Commit changes (git commit -m 'Add amazing feature')
Push to branch (git push origin feature/amazing-feature)
Open Pull Request
📄 License
This project is licensed under the MIT License.

👤 Author
Luka

GitHub: @luka55206
🙏 Acknowledgments
Built with .NET 9
Inspired by Clean Architecture principles
Thanks to the open-source community
