using Microsoft.OpenApi;
using ZeroFrame.API.Filters;
using ZeroFrame.API.Middleware;
using ZeroFrame.Infra.Data.Context;
using ZeroFrame.Infra.Data.Seed;
using ZeroFrame.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiErrorResultFilter>();
});
builder.Services.AddCors(options =>
{
    var configuredFrontendOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    var allowedFrontendOrigins = new[]
    {
        "http://localhost:3000",
        "http://127.0.0.1:3000",
        "http://localhost:5173",
        "http://127.0.0.1:5173",
        "http://localhost:5500",
        "http://127.0.0.1:5500",
        "https://localhost:3090"
    }.Concat(configuredFrontendOrigins)
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .ToArray();

    options.AddPolicy(name: "ZeroFrame_Frontend",
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
                    origin == "null"
                    || allowedFrontendOrigins.Contains(origin)
                    || Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                        && (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null, "Bearer"),
            new List<string>()
        }
    });
});
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var webRootPath = app.Environment.WebRootPath
        ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");

    Directory.CreateDirectory(webRootPath);
    await DevelopmentDataSeeder.SeedAsync(dbContext, webRootPath);
}

app.UseCors("ZeroFrame_Frontend");
app.UseExceptionMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
