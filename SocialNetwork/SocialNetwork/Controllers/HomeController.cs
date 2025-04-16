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

    public HomeController(ILogger<HomeController> logger, AppDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var allPosts = await _context.Posts
        .Include(n => n.User)
        .Include(n => n.Comments)
        .Include(n=> n.Likes)
        .Include(n => n.Favorites)
        .ThenInclude(n => n.User)
        .OrderByDescending(n => n.DateCreated)
        .ToListAsync();

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(PostVM post)
    {
        // Get the logged in user
        int loggedInUser = 1;

        // Create a new post
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            NrOfReports = 0,
            UserId = loggedInUser
        };

        // Check and save the image
        if (post.Image != null && post.Image.Length > 0)
        {
            string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (post.Image.ContentType.Contains("image"))
            {
                string rootFolderPathImages = Path.Combine(rootFolderPath, "images/uploaded");
                Directory.CreateDirectory(rootFolderPathImages);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                string filePath = Path.Combine(rootFolderPathImages, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await post.Image.CopyToAsync(stream);

                // Set the URL to the newPost object
                newPost.ImageUrl = "/images/uploaded/" + fileName;
            }
        }

        // Add the post to the database
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();

        // Redirect to the index page
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM) 
    {
        int loggedInUser = 1;
        // chek if the user has already liked the post
        var like = await _context.Likes.Where(n => n.UserId == loggedInUser && n.PostId == postLikeVM.PostId).FirstOrDefaultAsync();
        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
        else
        {
            var newlike = new Like()
            {
                PostId = postLikeVM.PostId,
                UserId = loggedInUser
            };
            await _context.Likes.AddAsync(newlike);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
        
    }

    [HttpPost]
    public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
    {
        int loggedInUser = 1;
        // chek if the user has already favorited the post
        var favorite = await _context.Favorites.Where(n => n.PostId == postFavoriteVM.PostId && n.UserId == loggedInUser).FirstOrDefaultAsync();
        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
        else
        {
            var newFavorite = new Favorite()
            {
                PostId = postFavoriteVM.PostId,
                UserId = loggedInUser
            };
            await _context.Favorites.AddAsync(newFavorite);
            await _context.SaveChangesAsync();
        }
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

        await _context.Comments.AddAsync(newComment);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        var commentDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == removeCommentVM.CommentId);

        if(commentDb != null)
        {
            _context.Comments.Remove(commentDb);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");

    }
    


}