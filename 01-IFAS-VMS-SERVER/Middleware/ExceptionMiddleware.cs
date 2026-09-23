namespace IFAS.Server.Middleware;
public sealed class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> log)
{ public async Task InvokeAsync(HttpContext ctx){try{await next(ctx);}catch(Exception ex){log.LogError(ex,"Unhandled server exception");ctx.Response.StatusCode=500;ctx.Response.ContentType="application/json";await ctx.Response.WriteAsJsonAsync(new{message="Internal server error."});}}}
