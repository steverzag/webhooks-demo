var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin()
	//.WithLifetime(ContainerLifetime.Persistent)
	.AddDatabase("webhooks");

var queue = builder.AddRabbitMQ("rabbitmq")
	.WithDataVolume()
	.WithManagementPlugin();

builder.AddProject<Projects.Webhooks_API>("webhooks-api")
	.WithReference(database)
	.WithReference(queue)
	.WaitFor(database)
	.WaitFor(queue);

builder.AddProject<Projects.Webhooks_Processing>("webhooks-processing")
	.WithReplicas(3)
	.WithReference(database)
	.WithReference(queue)
	.WaitFor(database)
	.WaitFor(queue);

builder.Build().Run();
