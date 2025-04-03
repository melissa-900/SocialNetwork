using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var allPosts = await _context.Posts.Include(n => n.User).OrderByDescending(n => n.DateCreated).ToListAsync();

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

        // Add the post to the database
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();

        // Redirect to the index page
        return RedirectToAction("Index");
    }
}