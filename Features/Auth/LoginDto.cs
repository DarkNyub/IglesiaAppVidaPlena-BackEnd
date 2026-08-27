namespace IglesiaBackend.Features.Auth;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username (used as login identifier)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Plain password (will be validated against stored hash)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Login response DTO
/// </summary>
public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // Info básica útil en frontend
    public int UserId { get; set; }
    public int MemberId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string SystemRole { get; set; } = string.Empty;
}
