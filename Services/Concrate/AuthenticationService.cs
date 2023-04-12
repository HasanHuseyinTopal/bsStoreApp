using AutoMapper;
using Azure.Core;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrate
{
    internal class AuthenticationService : IAuthenticationService
    {
        readonly UserManager<User> _userManager;
        readonly ILoggerService _loggerService;
        readonly IMapper _mapper;
        readonly IConfiguration _configuration;
        User user;

        public AuthenticationService(UserManager<User> userManager, ILoggerService loggerService, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _loggerService = loggerService;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IdentityResult> RegisterUser(UserRegisterationDTO userRegisterationModel)
        {
            var user = _mapper.Map<User>(userRegisterationModel);
            var result = await _userManager.CreateAsync(user, userRegisterationModel.Password);
            if (result.Succeeded)
            {
                var addToRole = await _userManager.AddToRolesAsync(user, userRegisterationModel.Roles);
                if (addToRole.Succeeded)
                {
                    return result;
                }
            }
            throw new Exception("User cant create");
        }
        public async Task<TokenDTO> CreateToken(bool pupuleExp)
        {
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF32.GetBytes(_configuration["JWT:SecretKey"]));
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new(
                issuer: _configuration["JWT:İssuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddMinutes(5),
                claims: await Claims(),
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials
                );
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var accessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            if (pupuleExp)
            {
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10);
            }
            await _userManager.UpdateAsync(user);
            return new TokenDTO()
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken
            };
        }
        public async Task<List<Claim>> Claims()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO)
        {
            user = await _userManager.FindByNameAsync(userForAuthenticationDTO.UserName);
            var result = (user != null && await _userManager.CheckPasswordAsync(user, userForAuthenticationDTO.Password));
            if (!result)
                _loggerService.Warn("Authentication failed !");
            return result;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSetting = _configuration.GetSection("Jwt");
            var tokenParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidAudience = jwtSetting["JWT:Audience"],
                ValidIssuer = jwtSetting["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(jwtSetting["SecretKey"]))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != tokenDTO.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new RefreshTokenBadRequestException();
            }
            this.user = user;
            return await CreateToken(false);
        }
    }
}
