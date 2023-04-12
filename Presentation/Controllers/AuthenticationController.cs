using EntityLayer.DTOs;
using EntityLayer.Entities.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        readonly IServiceManager serviceManager;

        public AuthenticationController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterationDTO userRegisterationDTO)
        {
            var result = await serviceManager.AuthenticationService.RegisterUser(userRegisterationDTO);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDTO userForAuthenticationDTO)
        {
            if (!await serviceManager.AuthenticationService.ValidateUser(userForAuthenticationDTO))
                return Unauthorized();
            return Ok(new
            {
                Token = await serviceManager.AuthenticationService.CreateToken(true)
            });
        }
        [HttpPost("Refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDTO)
        {
            var tokenDtoReturn = await serviceManager.AuthenticationService.RefreshToken(tokenDTO);
            return Ok(tokenDtoReturn);
        }
    }
}
