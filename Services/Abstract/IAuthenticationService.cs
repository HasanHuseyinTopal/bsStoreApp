using EntityLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserRegisterationDTO userRegisterationModel);
        Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO);
        Task<TokenDTO> CreateToken(bool puplateExp);
        Task<TokenDTO> RefreshToken(TokenDTO tokenDTO);
    }
}
