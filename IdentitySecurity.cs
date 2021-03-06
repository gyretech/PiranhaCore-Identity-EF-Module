﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Piranha.AspNetCore.Identity.EF
{
    public class IdentitySecurity : ISecurity
    {
        private readonly SignInManager<IdentityAppUser> _signInManager;
        private readonly UserManager<IdentityAppUser> _userManager;
        // Default Contructor
        public IdentitySecurity(Action<IdentityDbBuilder> options, SignInManager<IdentityAppUser> signInManager, UserManager<IdentityAppUser> userManager, PiranhaIdentityDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;

            var config = new IdentityDbBuilder();
            options?.Invoke(config);

            if (!config.Migrate) return;
            try
            {
                IdentityDbInitializer.Initialize(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (!config.Users.Any()) return;

            foreach (var appUser in config.Users)
            {
                var claims = new List<Claim>();

                if (appUser.Claims.Any())
                {
                    foreach (var claim in appUser.Claims)
                    {
                        claims.Add(new Claim(claim, claim));
                    }
                }

                var user = _userManager.FindByNameAsync(appUser.UserName).Result;

                if (user != null) continue;

                var created = _userManager.CreateAsync(appUser, appUser.Password).Result.Succeeded;

                if (created)
                {
                    var unused = _userManager.AddClaimsAsync(appUser, claims).Result.Succeeded;
                }
            }
        }


        public bool Authenticate(string username, string password)
        {
            var user = _userManager.FindByNameAsync(username).Result;

            if (user == null) return false;

            var result = _signInManager.CheckPasswordSignInAsync(user, password, false).Result;

            return result.IsNotAllowed;
        }

       

        public async Task<bool> SignIn(object context, string username, string password)
        {
            if (context is HttpContext)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    var user = ((HttpContext)context).User;

                    await ((HttpContext) context).SignInAsync(user);

                    return result.Succeeded;
                }
            }

            return false;
        }

        public async Task SignOut(object context)
        {
            if (context is DefaultHttpContext)
            {
                await _signInManager.SignOutAsync();

                await((DefaultHttpContext)context).SignOutAsync();
            }
        }

        public void SeedUser(IServiceProvider services, string userName, string password, string[] all)
        {
            using (var scope = services.CreateScope())
            {
                var svcs = scope.ServiceProvider;
                try
                {
                    List<Claim> claims = all.Select(claim => new Claim(claim, claim)).ToList();

                    if (!claims.Any()) return;

                    var userManager = svcs.GetRequiredService<UserManager<IdentityAppUser>>();

                    if (userManager == null) return;

                    var user = userManager.FindByNameAsync(userName).Result ?? new IdentityAppUser() { UserName = userName };

                    var result = userManager.CreateAsync(user, password).Result.Succeeded;

                    if (result)
                    {
                        var unused = userManager.AddClaimsAsync(user, claims).Result.Succeeded;
                    }
                    
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger>();
                    logger?.LogError(ex, "Error occured.");

                    throw;
                }
            }
        }

        
    }
}