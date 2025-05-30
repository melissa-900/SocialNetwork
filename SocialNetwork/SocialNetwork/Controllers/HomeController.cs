using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetwork.Data;
using SocialNetwork.Data.Helpers.Enums;
using SocialNetwork.Data.Models;
using SocialNetwork.Data.Services;
using SocialNetwork.ViewModels.Home;

namespace SocialNetwork.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDBContext _context;
    private readonly IPostService _postService;
    private readonly IFilesService _fileService;
    public HomeController(ILogger<HomeController> logger, AppDBContext context, IPostService postService, IFilesService fileService)
    {
        _logger = logger;
        _context = context;
        _postService = postService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        int loggeedInUser = 1;
        var allPosts = await _postService.GetAllPostAsync(loggeedInUser);

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

        await _postService.CreatePostAsync(newPost);

        // Redirect to the index page
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM) 
    {
        int loggedInUser = 1;
        await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUser);
        var post = await _postService.GetPostByIdAsync(postLikeVM.PostId);

        return PartialView("Home/_Post",post);
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
    {
        int loggedInUser = 1;
        await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUser);

        var post = await _postService.GetPostByIdAsync(postFavoriteVM.PostId);

        return PartialView("Home/_Post", post);

    }

    [HttpPost]
    public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
    {
        int loggedInUser = 1;
        await _postService.TogglePostVisibility(postVisibilityVM.PostId, loggedInUser);
        return RedirectToAction("Index");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
        await _postService.AddPostComentAsync(newComment);

        var post = await _postService.GetPostByIdAsync(postCommentVM.PostId);
        return PartialView("Home/_Post", post);
    }

    [HttpPost]
    public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
    {
        int loggedInUser = 1;

        await _postService.ReportPostAsync(postReportVM.PostId,loggedInUser);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        await _postService.RemovePostCommentAsync(removeCommentVM.CommentId);
        var post = await _postService.GetPostByIdAsync(removeCommentVM.PostId);

        return PartialView("Home/_Post", post);
    }

    [HttpPost]
    public async Task<IActionResult> PostDelete(PostDeleteVM postDeleteVM)
    {
        await _postService.RemovePostAsync(postDeleteVM.PostId);
        return  RedirectToAction("Index");
    }

}