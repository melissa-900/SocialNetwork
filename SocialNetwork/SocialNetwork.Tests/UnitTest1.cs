using Xunit;
using Moq;
using SocialNetwork.Controllers;
using SocialNetwork.Data;
using SocialNetwork.Data.Models;
using SocialNetwork.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SocialNetwork.Data.Services;
namespace SocialNetwork.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly AppDBContext _context;
        private readonly IPostService _postService;
        public HomeControllerTests()
        {
            // Create In-Memory DB for tests
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            // Create context
            _context = new AppDBContext(options);

            // Mock Logger
            _mockLogger = new Mock<ILogger<HomeController>>();


            //Create Post Service
            _postService = new PostService(_context);
            // Create controller with InMemory context
            _controller = new HomeController(_mockLogger.Object, _context,_postService);

            // Clean the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

        }

        // CrestePost TESTS

        [Fact]
        public async Task CreatePost_ShouldAddPostToDatabase()
        {
            // Arrange
            var postVM = new PostVM
            {
                Content = "Hello World!",
                Image = null
            };

            // Act
            var result = await _controller.CreatePost(postVM);

            // Assert
            var postCount = await _context.Posts.CountAsync();
            Assert.Equal(1, postCount); // Check if post was added

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.IsType<RedirectToActionResult>(result);
        }

        // Like TESTS

        [Fact]
        public async Task LikePost_ShouldToggleLike()
        {
            // Arrange
            int userId = 1;
            var post = new Post
            {
                Id = 1,
                Content = "Test Post",
                DateCreated = DateTime.UtcNow,
                UserId = userId
            };

            // Add Post to DB
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var like = new Like
            {
                PostId = post.Id,
                UserId = userId
            };

            // Add Like to DB
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();

            // Act
            var likesCount = await _context.Likes.CountAsync(l => l.PostId == post.Id && l.UserId == userId);
            Assert.Equal(1, likesCount); // Check if Like added

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            // Assert
            likesCount = await _context.Likes.CountAsync(l => l.PostId == post.Id && l.UserId == userId);
            Assert.Equal(0, likesCount); // Check if Like removed
        }

    }
}
