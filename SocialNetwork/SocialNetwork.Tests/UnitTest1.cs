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

namespace SocialNetwork.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly AppDBContext _context;

        public HomeControllerTests()
        {
            // Create In-Memory DB for tests
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            // Create context
            _context = new AppDBContext(options);

            // Mock Logger
            _mockLogger = new Mock<ILogger<HomeController>>();

            // Create controller with InMemory context
            _controller = new HomeController(_mockLogger.Object, _context);
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
            Assert.IsType<RedirectToActionResult>(result); // Ensure it redirects to Index page
        }
    }
}
