var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WebApi>("webapi");

builder.AddProject<Projects.ParkingClient>("parkingclient", launchProfileName: null)
    .WithHttpEndpoint(port: 5200, name: "http")
    .WithHttpsEndpoint(port: 7200, name: "https")
    .WithReference(api);

builder.Build().Run();
