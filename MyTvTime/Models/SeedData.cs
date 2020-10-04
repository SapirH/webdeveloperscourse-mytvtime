using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MyTvTime.Data;


namespace MyTvTime.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new UserContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<UserContext>>()))
            {
                // Look for any movies.
                if (context.User.Any())
                {
                    return;   // DB has been seeded
                }

                context.User.AddRange(
                    new User
                    {
                       Name = "Itay"
                    },
                    new User
                    {
                        Name = "Sapir"
                    },
                    new User
                    {
                        Name = "Liav"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}