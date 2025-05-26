using Microsoft.AspNetCore.Http;
using SocialNetwork.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostAsync(int userId);
        Task<Post> CreatePostAsync(Post post, IFormFile image);
        Task RemovePostAsync(int postId);
        Task AddPostComentAsync(Comment comment);
        Task RemovePostCommentAsync(int commentId);
        Task TogglePostLikeAsync(int postId, int userId);
        Task TogglePostFavoriteAsync(int postId, int userId);
        Task ReportPostAsync(int postId, int userId);
        Task TogglePostVisibility(int postId, int userId);
    }
}
