﻿using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Controllers
{
    [Authorize(Roles="Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            List<User> users = getUsersFromFile(file.FileName);
            foreach (var item in users)
            {
                var userCheck = await this._userManager.FindByEmailAsync(item.Email);
                if (userCheck == null)
                {
                    var user = new AppUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new Cart()
                    };
                    var result = await this._userManager.CreateAsync(user, item.Password);
                    if (result.Succeeded)
                    {
                        await this._userManager.AddToRoleAsync(user, item.Role);
                    }
                }
                else
                {
                    continue;
                }
            }
            return RedirectToAction("Index");
        }

        private List<User> getUsersFromFile(string fileName)
        {
            List<User> users = new List<User>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\{fileName}";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),
                            Role = reader.GetValue(3).ToString()
                        });
                    }
                }
            }
            return users;
        }
    }
}
