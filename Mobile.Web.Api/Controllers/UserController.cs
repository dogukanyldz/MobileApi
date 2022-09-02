using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mobile.Dal.Services.Abstract;
using Mobile.Entities;
using Mobile.Entities.Entities;
using Mobile.Web.Api.RequestModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        public UserController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            var checkUser = await _userManager.FindByEmailAsync(user.Email);
            if (checkUser is null)
                return Ok(new {existUser =false});


            var result = await _userManager.CheckPasswordAsync(checkUser, user.Password);

            if (!result)
                return Ok( new { existUser = false });


            var userRoles = await _userManager.GetRolesAsync(checkUser);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, checkUser.Id),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.CreateToken(authClaims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            checkUser.RefreshToken = refreshToken;
            checkUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(checkUser);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                existUser = true
            });

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel user)
        {
            var exist = await _userManager.FindByEmailAsync(user.Email);
            if (exist is not null)
                return Ok(new { result = false });

            var app_user = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(app_user, user.Password);

            if (!result.Succeeded)
                return Ok(new { result = false });

            return Ok(new { result = true });

        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)          
                return BadRequest("Invalid client request");
            

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = _tokenService.ValidateExpiredToken(accessToken);
            if (principal == null)
                return Unauthorized("Invalid access token or refresh token");
            

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
                                                                                        //07.08.2022            //08.07.2022        
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized("Invalid access token or refresh token");
            }

            var newAccessToken = _tokenService.CreateToken(principal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            
           
            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }



    }
}
