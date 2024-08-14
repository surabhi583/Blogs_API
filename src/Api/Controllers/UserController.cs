using Azure;
using Blogs.Core.Services;
using Blogs.Core.Services.Interfaces;

namespace Blogs.Api.Controllers;

[Route("[controller]")]
[Authorize]
public class UserController(IUserHandler userHandler, ILogger<UserController> _logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserModel<UserDto>>> GetUser(CancellationToken cancellationToken)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userHandler.GetAsync(username, cancellationToken);
        return Ok(new UserModel<UserDto>(user));
    }

    [HttpGet("users")]
    public async Task<ActionResult<UsersResponse>> GetUsers([FromQuery] UsersQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting list of users...");
            var usersResp = await userHandler.GetUsersAsync(query, cancellationToken);
            var result = UsersMapper.MapFromUsers(usersResp);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex;
        }
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<UserModel<UserDto>>> GetUserByEmailAsync(string email,
        CancellationToken cancellationToken)
    {
        var user = await userHandler.GetUserByEmailAsync(email, cancellationToken);
        return Ok(new UserModel<UserDto>(user));
    }

    [HttpDelete("{email}")]
    public async Task<ActionResult<UserModel<UserDto>>> DeleteByEmailAsync(string email,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting user by email...");
            await userHandler.DeleteAsync(email, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex;
        }
    }

    [HttpPut]
    public async Task<ActionResult<UserModel<UserDto>>> UpdateUser(
        RequestEnvelope<UserModel<UpdatedUserDto>> request, CancellationToken cancellationToken)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userHandler.UpdateAsync(username, request.Body.User, cancellationToken);

        return Ok(new UserModel<UserDto>(user));
    }
}
