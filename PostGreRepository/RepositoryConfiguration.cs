using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace PostGreRepository
{
    public static class RepositoryConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["PostgreSql:ConnectionString"];
            var dbPassword = configuration["PostgreSql:DbPassword"];

            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Password = dbPassword
            };

            services.AddDbContext<JustOneContext>(options => options
                .UseNpgsql(builder.ConnectionString)
            );

            services.AddScoped<RepositoryInterface.JustOne.ISessionRepository, JustOne.SessionRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IPlayerRepository, JustOne.PlayerRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IRoundRepository, JustOne.RoundRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseRepository, JustOne.ResponseRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IPlayerStatusRepository, JustOne.PlayerStatusRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseVoteRepository, JustOne.ResponseVoteRepository>();
        }
    }
}
