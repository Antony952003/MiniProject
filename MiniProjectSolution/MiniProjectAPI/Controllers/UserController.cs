using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Login;
using MovieBookingAPI.Models.DTOs.Register;
using System.Security.Claims;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userService.Login(userLoginDTO);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    //_logger.LogCritical("User not authenticated");
                    return Unauthorized(new ErrorModel(401, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterOutputDTO>> Register(RegisterInputDTO userDTO)
        {
            try
            {
                RegisterOutputDTO result = await _userService.Register(userDTO);
                if (result == null)
                {
                    return Unauthorized(new ErrorModel(400, "Passwords doesn't Match"));
                }
                return Ok(result);
            }
            catch (UnableToRegisterException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
