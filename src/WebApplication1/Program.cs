using WebApplication1.Hubs;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Add background service
builder.Services.AddHostedService<MessageSenderService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5208", 
                "http://localhost:5209", 
                "http://localhost:5210",
                "http://localhost:9951",
                "https://localhost:7208",
                "https://localhost:7209"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for SignalR
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowBlazorApp");

app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<MessageHub>("/messagehub");

app.Run();