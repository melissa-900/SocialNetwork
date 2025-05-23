using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Services
{
    public class PostService : IPostsService
    {
        private readonly AppDbContext _context;
        public PostService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Post>> GetAllPostsAsync(int loggedInUserId)
        {
            var allPosts = await _context.Posts
              .Include(n => n.User)
              .Include(n => n.Comments)
              .ThenInclude(n => n.User)
              .OrderByDescending(n => n.DateCreated)
              .ToListAsync();
            return allPosts;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }
        public async Task RemovePostAsync(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);

            if (postDb != null)
            {
                //_context.Posts.Remove(postDb);
                postDb.IsDeleted = true; 
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }
        public async Task RemovePostCommentAsync(int commentId)
        {
            var commentDb = _context.Comments.FirstOrDefault(n => n.Id == commentId);

            if (commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
        }

        public Task TogglePostLikeAsync(int postId, int userId)
        {
            throw new NotImplementedException();
        }
        public Task TogglePostFavoriteAsync(int postId, int userId)
        {
            throw new NotImplementedException();
        }
        public async Task ReportPostAsync(int postId, int userId)
        {
            var new Report = new Report()
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();
        }
    }
