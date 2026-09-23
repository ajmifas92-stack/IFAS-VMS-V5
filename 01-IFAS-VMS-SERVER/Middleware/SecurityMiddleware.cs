namespace IFAS.Server.Middleware;
public sealed class SecurityMiddleware(RequestDelegate next)
{ public async Task InvokeAsync(HttpContext ctx){ctx.Response.Headers["X-Content-Type-Options"]="nosniff"; ctx.Response.Headers["X-Frame-Options"]="DENY"; ctx.Response.Headers["Referrer-Policy"]="no-referrer"; ctx.Response.Headers["Permissions-Policy"]="camera=(), microphone=()"; await next(ctx);}}
