using Boundary.ApplicationServicesConfig;
using Boundary.DALConfig;
using Boundary.ExternalConfig;
using DAL.Assembly;
using eZbori.Sender.Workers;
using MediatR;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<InviteEmailWorker>();
builder.Services.AddHostedService<PasswordResetEmailWorker>();
builder.Services.AddHostedService<ImportWorker>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing DefaultConnection configuration");

builder.Services.AddSql(connectionString);
builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.AddHttpClients();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DalAssembly).Assembly));

var host = builder.Build();
await host.RunAsync();
