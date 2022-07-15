using System.Text.Json.Serialization;
using dp.api.Authorization;
using dp.api.Helpers;
using dp.api.Services;
using Microsoft.Extensions.Options;


//Auth is taken a lot from this blog post below, but I added on to this a bit
//https://jasonwatmore.com/post/2022/02/18/net-6-role-based-authorization-tutorial-with-example-api

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    //services.AddDbContext<DataContext>();
    services.AddCors();
    services.AddHealthChecks();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    //services.AddScoped<IUserService, UserService>();
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();
    app.MapHealthChecks("/health");
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}


app.Run();