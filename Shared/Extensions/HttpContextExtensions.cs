namespace IglesiaBackend.Shared.Extensions;

public static class HttpContextExtensions
{
    public static int? GetUserId(this HttpContext context)
    {
        // Leemos directamente de Items, porque el UserContextMiddleware ya lo puso ahí.
        if (context.Items.TryGetValue("UserId", out var value) && value is int userId)
        {
            return userId;
        }

        return null;
    }
}