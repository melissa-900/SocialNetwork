using SocialNetwork.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Helpers
{
   public static class DbInitializer
    {

        public static async Task SeedAsync(AppDBContext appDBContext)
        {
            if(!appDBContext.Users.Any() && !appDBContext.Posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "Lorem ipsum",
                    ProfilePictureUrl = "#"
                };
                await appDBContext.Users.AddAsync(newUser);
                await appDBContext.SaveChangesAsync();
                var newPostWithoutImage = new Post()
                {
                    Content = "First post, wich was loaded from database and was created by test user",
                    ImageUrl = " ",
                    NrOfReports =0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UserId = newUser.Id
                };
                var newPostWithImage = new Post()
                {
                    Content = "First post, wich was loaded from database and was created by test user",
                    ImageUrl = "https://i.etsystatic.com/28810262/r/il/2fc5e0/5785166966/il_1588xN.5785166966_nvy4.jpg",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UserId = newUser.Id
                };

                await appDBContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
