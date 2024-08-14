namespace Blogs.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController(IUserHandler userHandler, ILogger<UsersController> _logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserModel<UserDto>>> Register(
        RequestEnvelope<UserModel<NewUserDto>> request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Registering user to Blog...");
            var user = await userHandler.CreateAsync(request.Body.User, cancellationToken);
            return Ok(new UserModel<UserDto>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex;
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserModel<UserDto>>> Login(
        RequestEnvelope<UserModel<LoginUserDto>> request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Logging to Blog...");
            var user = await userHandler.LoginAsync(request.Body.User, cancellationToken);
            return Ok(new UserModel<UserDto>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex;
        }
    }
}
