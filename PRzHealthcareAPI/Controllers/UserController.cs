using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("account")]
    [ApiController]

    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto dto)
        {
            _userService.Register(dto);
            return Ok();
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            var loggedUser = _userService.GenerateToken(dto);
            return Ok(loggedUser);
        }

        [HttpPost("changepassword")]
        [Authorize]
        public ActionResult ChangePassword([FromBody] LoginUserDto dto)
        {
            _userService.ChangePassword(dto);
            return Ok();
        }

    }
}
