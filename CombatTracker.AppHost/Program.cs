var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.CombatTracker_ApiService>("apiservice");

builder.AddProject<Projects.CombatTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
