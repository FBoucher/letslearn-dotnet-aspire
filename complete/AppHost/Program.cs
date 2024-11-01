var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithRedisCommander();

var api = builder.AddProject<Projects.Api>("api")
                 .WithReference(cache)
                 .WithEnvironment(context =>
                    {
                        context.EnvironmentVariables["OTEL_SERVICE_NAME"] = "myweatherapi";
                        context.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:4317";
                        context.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "grpc";
                        context.EnvironmentVariables["OTEL_RESOURCE_ATTRIBUTES"] = "deployment.environment=docker,host.name=otelcol-docker";
                        context.EnvironmentVariables["DD_AGENT_HOST"] = "datadog";
                    });

var web = builder.AddProject<Projects.MyWeatherHub>("myweatherhub")
                 .WithReference(api)
                 .WithExternalHttpEndpoints()
                 .WithEnvironment(context =>
                    {
                        context.EnvironmentVariables["OTEL_SERVICE_NAME"] = "myweatherhub";
                        context.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:4317";
                        context.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "grpc";
                        context.EnvironmentVariables["OTEL_RESOURCE_ATTRIBUTES"] = "deployment.environment=docker,host.name=otelcol-docker";
                        context.EnvironmentVariables["DD_AGENT_HOST"] = "datadog";
                    });

builder.Build().Run();
