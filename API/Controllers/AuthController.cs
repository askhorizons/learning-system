using API.Models;
using API.Services;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(
            IConfiguration config,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            TokenService tokenService)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        #region Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null) return Unauthorized("Invalid email");

                if (!user.EmailConfirmed) return Unauthorized("Email not confirmed");

                var result = await _signInManager.CheckPasswordSignInAsync(
                    user: user, 
                    password: model.Password, 
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await SetRefreshToken(user);
                    return CreateUserObject(user);
                }

                return Unauthorized("Invalid password");
            }

            return BadRequest(ModelState);
        }
        #endregion

        #region Create User Object
        private UserModel CreateUserObject(AppUser user)
        {
            return new UserModel
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
                Image = string.Empty
            };
        }
        #endregion

        #region Set Refresh Token
        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
        #endregion

        #region Refresh Token
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserModel>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users
                .Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            return CreateUserObject(user);
        }

        #endregion
    }
}
