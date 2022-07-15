namespace dp.api.Authorization;

using Microsoft.Extensions.Options;
using dp.api.Helpers;
using dp.api.Services;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            var userFromDb = await userService.GetById(userId.Value);  //probably this should be cached
            context.Items["UserDb"] = userFromDb;
        }


        await _next(context);
    }
}