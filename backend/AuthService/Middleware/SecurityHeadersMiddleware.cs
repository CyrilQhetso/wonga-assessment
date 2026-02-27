namespace AuthService.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent clickjacking
        headers["X-Frame-Options"] = "DENY";

        headers["X-Content-Type-Options"] = "nosniff";

        headers["X-XSS-Protection"] = "1; mode=block";

        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        headers["Content-Security-Policy"] = "default-src 'self'; connect-src 'self' http://localhost:5000";

        headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload";

        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        await _next(context);
    }
}

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}