using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetwork.Data;
using SocialNetwork.Data.Models;
using SocialNetwork.ViewModels.Home;

namespace SocialNetwork.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDBContext _context;
    private readonly IPostsService _postsService;
    private readonly IFileService _fileService

    public HomeController(ILogger<HomeController> logger, AppDBContext context, IPostsService _postsService)
    {
        _logger = logger;
        _context = context;
        _postsService = postsService;
    }

    public async Task<IActionResult> Index()
    {
        int loggedInUserId = 1;
        var allPosts = await _postsService.GetAllPostsAsync(loggedInUserId);

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(PostVM post)
    {
        // Get the logged in user
        int loggedInUser = 1;

        var imageUploadPath = await _fileService.UploadImageAsync(post.Image, ImageFileType.PostImage);

        // Create a new post
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = imageUploadPath,
            NrOfReports = 0,
            UserId = loggedInUser
        };

        await _postsService.CreatePostAsync(newPost);

        // Redirect to the index page
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
    {
        int loggedInUser = 1;

        //Creat a post object
        var newComment = new Comment()
        {
            UserId = loggedInUser,
            PostId = postCommentVM.PostId,
            Content = postCommentVM.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        await _postsService.AddPostCommentAsync(newComment);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        await _postsService.RemovePostCommentAsync(removeCommentVM.CommentId);

        return RedirectToAction("Index");

    }
    
}