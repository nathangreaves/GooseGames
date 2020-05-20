using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using MSSQLRepository.Contexts;

namespace MSSQLRepository
{
    public static class RepositoryConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //var connectionString = configuration["MSSQL:ConnectionString"];
            //var dbPassword = configuration["MSSQL:DbPassword"];

            //var builder = new SqlConnectionStringBuilder(connectionString)
            //{
            //    Password = dbPassword
            //};

            services.AddDbContext<JustOneContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddDbContext<FujiContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddDbContext<CodenamesContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddDbContext<WerewordsContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddScoped<RepositoryInterface.JustOne.ISessionRepository, JustOne.SessionRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IPlayerRepository, JustOne.PlayerRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IRoundRepository, JustOne.RoundRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseRepository, JustOne.ResponseRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IPlayerStatusRepository, JustOne.PlayerStatusRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseVoteRepository, JustOne.ResponseVoteRepository>();

            services.AddScoped<RepositoryInterface.Fuji.ISessionRepository, Fuji.SessionRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IPlayerRepository, Fuji.PlayerRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IHandCardRepository, Fuji.HandCardRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IDeckCardRepository, Fuji.DeckCardRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IDiscardedCardRepository, Fuji.DiscardedCardRepository>();

            services.AddScoped<RepositoryInterface.Codenames.ICodenamesRepository, Codenames.CodenamesRepository>();

            services.AddScoped<RepositoryInterface.Werewords.ISessionRepository, Werewords.SessionRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerRepository, Werewords.PlayerRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IRoundRepository, Werewords.RoundRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerRoundInformationRepository, Werewords.PlayerRoundInformationRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerVoteRepository, Werewords.PlayerVoteRepository>();
        }
    }
}
