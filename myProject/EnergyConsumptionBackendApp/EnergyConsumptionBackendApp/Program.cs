using EnergyConsumptionBackendApp.API.Services.Interfaces;
using EnergyConsumptionBackendApp.API.Services;
using EnergyConsumptionBackendApp.Data;
using Microsoft.EntityFrameworkCore;
using EnergyConsumptionBackendApp.Data.Repositories.Interfaces;
using EnergyConsumptionBackendApp.Data.Repositories;
using EnergyConsumptionBackendApp.Core.Models;
using System.Net;
using EnergyConsumptionBackendApp.Core.Constants;
using EnergyConsumptionBackendApp.API.GrpcServices;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")
                    ));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IEnergyService, EnergyService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Device>, Repository<Device>>();
builder.Services.AddScoped<IRepository<Energy>, Repository<Energy>>();


builder.Services.AddGrpc();

var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


// Configure the HTTP request pipeline.

app.UseWebSockets();

app.UseRouting();
app.UseGrpcWeb();
//Enable Authentication
app.UseAuthentication();
app.UseAuthorization(); //<< This needs to be between app.UseRouting(); and app.UseEndpoints();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<ChatService>().EnableGrpcWeb();
});

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        var buffer = new byte[100];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string message = System.Text.Encoding.UTF8.GetString(buffer);
        string username = "";
        foreach (char c in message)
        {
            if ((c > 'a' && c < 'z') || (c > 'A' && c > 'Z') || (c > '0' && c > '9') || c == '_')
            {
                username += c;
            }
        }
        
        if (Connections.websocketConnections.TryAdd(username, webSocket))
        {
            Console.WriteLine("Websocket connection was added to the dictionary for: " + username);
        }

        while(true)
        {

        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();