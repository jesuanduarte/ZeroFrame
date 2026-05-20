using Microsoft.OpenApi;
using ZeroFrame.API.Filters;
using ZeroFrame.API.Middleware;
using ZeroFrame.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiErrorResultFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ZeroFrame_Frontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://127.0.0.1:3000",
                    "http://localhost:5173",
                    "http://127.0.0.1:5173",
                    "http://localhost:5500",
                    "http://127.0.0.1:5500",
                    "https://localhost:3090")
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
});
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ZeroFrame_Frontend");
app.UseExceptionMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
