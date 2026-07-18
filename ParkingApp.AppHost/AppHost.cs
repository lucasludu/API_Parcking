var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WebApi>("webapi");

builder.AddProject<Projects.ParkingClient>("parkingclient")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
