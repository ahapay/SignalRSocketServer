using SignalRSocketServer.Hubs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add SignalR service
builder.Services.AddSignalR();

// Add CORS policy to allow SignalR connections from any origin
// In production, you should restrict this to specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:8080",
                  "http://127.0.0.1:8080",
                  "https://localhost:8080",
                  "https://127.0.0.1:8080",
                  "http://localhost:3000",
                  "http://127.0.0.1:3000",
                  "https://localhost:3000",
                  "https://127.0.0.1:3000",
                  "http://localhost:5173",
                  "http://127.0.0.1:5173",
                  "https://localhost:5173",
                  "https://127.0.0.1:5173",
                  "http://localhost:4200",
                  "http://127.0.0.1:4200",
                  "https://localhost:4200",
                  "https://127.0.0.1:4200",
                  "http://localhost:5500",
                  "http://127.0.0.1:5500",
                  "https://localhost:5500",
                  "https://127.0.0.1:5500"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("CorsPolicy");

app.UseAuthorization();

// Map controllers for REST API endpoints
app.MapControllers();

// Map SignalR hub endpoint
// This creates the /hub endpoint that clients will connect to
app.MapHub<NotificationHub>("/notificationHub").RequireCors("CorsPolicy");

// Serve static files for the test client
app.UseStaticFiles();

// Redirect root to Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.Run();
