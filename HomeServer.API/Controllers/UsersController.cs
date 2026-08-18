using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Users;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Application.Interfaces.UserSession;
using HomeServer.Domain.Enums;
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

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createDto)
        {
            var result = await _usersService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _usersService.GetMyProfileAsync(_currentUserService.UserId);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> GetUsersList([FromQuery] PaginationRequestDto paginationDto)
        {
            var result = await _usersService.GetAllAsync(paginationDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{userId}/for-edit")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> GetUserForEdit([FromRoute] Guid userId)
        {
            var result = await _usersService.GetForEditAsync(userId);

            return result.ToActionResult(this);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, UpdateUserDto updateDto)
        {
            var result = await _usersService.UpdateAsync(userId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpPut("{userId}/password")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> UpdateUserPassword([FromRoute] Guid userId, UpdateUserPasswordDto updateUserPasswordDto)
        {
            var result = await _usersService.UpdatePasswordAsync(userId, updateUserPasswordDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{userToDeleteId}")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> DeleteUser(Guid userToDeleteId)
        {
            var result = await _usersService.DeleteAsync(userToDeleteId, _currentUserService.UserId);

            return result.ToActionResult(this);
        }
    }
}
