using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Echo.Api.Data;
using Echo.Api.Models;
using Echo.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
               "http://localhost:5000",
               "https://localhost:5001",
               "http://localhost:7000",
               "https://localhost:7211",
               "http://localhost:5134")
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Echo API V1");
});

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapGroup("/identity").MapIdentityApi<IdentityUser>();

app.MapGet("/api/users", async (UserManager<IdentityUser> userManager, HttpContext context) =>
{
    var currentUserId = userManager.GetUserId(context.User);
    return await userManager.Users
        .Where(u => u.Id != currentUserId)
        .Select(u => new { u.Id, u.UserName, u.Email })
        .ToListAsync();
}).RequireAuthorization();

app.MapGet("/api/chat/history/{otherUserId}", async (AppDbContext db, string otherUserId, UserManager<IdentityUser> userManager, HttpContext context) =>
{
    var currentUserId = userManager.GetUserId(context.User);

    if (string.IsNullOrEmpty(currentUserId)) return Results.Unauthorized();

    var history = await db.ChatMessages
        .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
        .OrderBy(m => m.SentAt)
        .ToListAsync();

    return Results.Ok(history);
}).RequireAuthorization();

app.MapHub<EchoHub>("/echohub");

app.Run();