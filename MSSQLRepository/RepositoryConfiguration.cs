﻿using Microsoft.Extensions.DependencyInjection;
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

            services.AddDbContext<GlobalContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

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

            services.AddDbContext<LetterJamContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddDbContext<AvalonContext>(options => options
                .UseSqlServer(configuration["ConnectionStrings:MSSQL"])
            );

            services.AddScoped<RepositoryInterface.Global.ISessionRepository, Global.SessionRepository>();
            services.AddScoped<RepositoryInterface.Global.IPlayerRepository, Global.PlayerRepository>();

            services.AddScoped<RepositoryInterface.JustOne.IGameRepository, JustOne.GameRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IRoundRepository, JustOne.RoundRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseRepository, JustOne.ResponseRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IPlayerStatusRepository, JustOne.PlayerStatusRepository>();
            services.AddScoped<RepositoryInterface.JustOne.IResponseVoteRepository, JustOne.ResponseVoteRepository>();

            services.AddScoped<RepositoryInterface.Fuji.IGameRepository, Fuji.GameRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IPlayerInformationRepository, Fuji.PlayerInformationRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IHandCardRepository, Fuji.HandCardRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IDeckCardRepository, Fuji.DeckCardRepository>();
            services.AddScoped<RepositoryInterface.Fuji.IDiscardedCardRepository, Fuji.DiscardedCardRepository>();

            services.AddScoped<RepositoryInterface.Codenames.ICodenamesRepository, Codenames.CodenamesRepository>();

            services.AddScoped<RepositoryInterface.Werewords.IRoundRepository, Werewords.RoundRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerRoundInformationRepository, Werewords.PlayerRoundInformationRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerVoteRepository, Werewords.PlayerVoteRepository>();
            services.AddScoped<RepositoryInterface.Werewords.IPlayerResponseRepository, Werewords.PlayerResponseRepository>();

            services.AddScoped<RepositoryInterface.LetterJam.IGameRepository, LetterJam.GameRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IPlayerStateRepository, LetterJam.PlayerStateRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.ILetterCardRepository, LetterJam.LetterCardRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IRoundRepository, LetterJam.RoundRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.INonPlayerCharacterRepository, LetterJam.NonPlayerCharacterRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IClueRepository, LetterJam.ClueRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IClueVoteRepository, LetterJam.ClueVoteRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IClueLetterRepository, LetterJam.ClueLetterRepository>();
            services.AddScoped<RepositoryInterface.LetterJam.IFinalWordLetterRepository, LetterJam.FinalWordLetterRepository>();

            services.AddScoped<RepositoryInterface.Avalon.IGameRepository, Avalon.GameRepository>();
            services.AddScoped<RepositoryInterface.Avalon.IGameRoleRepository, Avalon.GameRoleRepository>();
            services.AddScoped<RepositoryInterface.Avalon.IPlayerIntelRepository, Avalon.PlayerIntelRepository>();
            services.AddScoped<RepositoryInterface.Avalon.IPlayerStateRepository, Avalon.PlayerStateRepository>();
        }
    }
}
