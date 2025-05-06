using ExternalMessageHandling.Interfaces;
using ExternalMessageHandling.Services;
using ExternalMessageHandling.Services.DataEvent;
using ExternalMessaging.Interfaces;
using ExternalMessaging.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// remote messaging service initialization, registers the client manager
builder.Services.AddSingleton<IDynamicDataClientService, ClientServiceProxy>();
builder.Services.AddSingleton<IDynamicDataClientManager, DynamicDataClientManager>();

// initializes the dynamic data  client within hosted services when the application starts
builder.Services.AddHostedService<DynamicDataClientInitializer>();

// ReSharper disable once RedundantTypeArgumentsOfMethod
builder.Services.AddSingleton<IDataDynamicService>(provider =>
{
    var manager = provider.GetRequiredService<IDynamicDataClientManager>();
    return manager.GetDataDynamicService();
});

// ReSharper disable once RedundantTypeArgumentsOfMethod
builder.Services.AddSingleton<IMessageSenderService>(provider =>
{
    var manager = provider.GetRequiredService<IDynamicDataClientManager>();
    return manager.GetMessageSenderService();
});

// register background service for entry updates, the application ensures that
// this service will be started automatically when the application starts and will run in the background
builder.Services.AddHostedService<DynamicDataService>();

builder.Services.AddSingleton<DataEventService>();
builder.Services.AddSingleton<DynamicDataSubscriptionManager>();

builder.Services.AddControllers();
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

app.Services.GetRequiredService<IDynamicDataClientManager>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<DataEventHub>("/dataEventHub");
});

app.Run();
