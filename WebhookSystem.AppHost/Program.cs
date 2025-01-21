var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin()
	.WithLifetime(ContainerLifetime.Persistent)
	.AddDatabase("webhooks");

builder.AddProject<Projects.Webhooks_API>("webhooks-api")
	.WithReference(database)
	.WaitFor(database);

builder.Build().Run();
