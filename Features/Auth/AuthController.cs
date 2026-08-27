using IglesiaBackend.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IglesiaBackend.Features.Auth;

[ApiController]
[Route("api/auth")]
[Consumes("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user and returns JWT token data
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Aquí capturamos "Usuario suspendido..." o "Credenciales inválidas"
            // Devolvemos 400 Bad Request con el mensaje para que Flutter lo muestre.
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("generate-pass-hash")]
    //[Authorize]
    public IActionResult GeneratePassHash(string password)
    {
        // Instanciamos el hasher
        var hasher = new PasswordHasher<User>();

        // Generamos el hash real compatible con .NET Core Identity
        // El primer parámetro (user) puede ser null o un new User() genérico para este caso
        var hashReal = hasher.HashPassword(new User(), password);

        return Ok(new { Password = password, HashValidoParaBD = hashReal });
    }
}
