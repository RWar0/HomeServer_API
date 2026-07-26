using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Users;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Application.Interfaces.UserSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IUsersService usersService, ICurrentUserService currentUserService)
        {
            _usersService = usersService;
            _currentUserService = currentUserService;
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<UserMyProfileDto>> GetMyProfile()
        {
            var result = await _usersService.GetUserMyProfile(_currentUserService.UserId);

            return result.ToActionResult(this);
        }
    }
}
