using AiAssaultArena.Api.Hubs;
using AiAssaultArena.Api.Services;
using Microsoft.AspNetCore.ResponseCompression;

namespace AiAssaultArena.Api;

public class ApiHost
{
    public static WebApplication Initialize(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSignalRSwaggerGen();
        });

        builder.Services.AddSignalR();

        builder.Services.AddSingleton<MatchService>();
        builder.Services.AddSingleton<MatchRepository>();

        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();
        app.UseResponseCompression();

        app.MapControllers();
        app.MapHub<MatchHub>("/match");

        app.Run();
        return app;
    }
}
