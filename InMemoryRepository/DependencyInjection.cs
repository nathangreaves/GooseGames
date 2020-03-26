using InMemoryRepository.Werewords.Game;
using Microsoft.Extensions.DependencyInjection;
using RepositoryInterface.Werewords.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryRepository
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWerewordsGameRepository, WerewordsGameRepository>();
        }
    }
}
