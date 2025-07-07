

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.File("logs/server.txt", rollingInterval: RollingInterval.Day));

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register message handlers
builder.Services.Scan(scan => scan
            .FromAssembliesOf(typeof(LoginHandler))
            .AddClasses(classes => classes.AssignableTo(typeof(IMessageHandler)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

// Register message handler factory
builder.Services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

//Use port 5000 with localhost binding
builder.WebHost.UseUrls("http://localhost:5000");

//Rate Limiting Configuration
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("device", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Headers["DeviceId"].ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Register middleware as scoped
builder.Services.AddScoped<WebSocketMiddleware>();

var app = builder.Build();

// Initialize database using DbInitializer
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
}

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.UseHttpsRedirection();
app.UseRateLimiter();

app.Run();