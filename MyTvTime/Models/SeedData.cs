﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MyTvTime.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new Data.TVContext(serviceProvider.GetRequiredService<DbContextOptions<Data.TVContext>>()))
            {
                if (context.User.Count() == 0)
                {
                    context.User.AddRange(
                    new User
                    {
                        username = "itaygolan88",
                        password = "Aa123123",
                        email = "itaygolan88@gmail.com",
                        sex = "Male",
                        country = "Kenya",
                        birthdate = new DateTime(1996, 11, 27),
                        language = "English",
                        isAdmin = true
                    },
                     new User
                     {
                         username = "Sapir",
                         password = "Aa123123",
                         email = "sapir@gmail.com",
                         sex = "Female",
                         country = "Malta",
                         birthdate = new DateTime(1996, 11, 27),
                         language = "German",
                         isAdmin = true
                     },
                      new User
                      {
                          username = "Liav",
                          password = "Aa123123",
                          email = "liav@gmail.com",
                          sex = "Male",
                          country = "Ghana",
                          birthdate = new DateTime(1996, 11, 27),
                          language = "French",
                          isAdmin = true
                      },
                      new User
                      {
                          username = "user",
                          password = "user",
                          email = "user@gmail.com",
                          sex = "Male",
                          country = "Ghana",
                          birthdate = new DateTime(1996, 11, 27),
                          language = "French",
                          isAdmin = false
                      });

                    context.SaveChanges();
                }
            }
        }
    }
}