var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

builder.AddProject<Projects.CombatTracker_WebAssembly>("webfrontend")
    .WithExternalHttpEndpoints();

builder.Build().Run();