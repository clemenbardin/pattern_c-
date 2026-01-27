using MonApp.API.Services;

namespace MonApp.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthService _authService;

    public JwtMiddleware(RequestDelegate next, AuthService authService)
    {
        _next = next;
        _authService = authService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            var principal = _authService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }
}
