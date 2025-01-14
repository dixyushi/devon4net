﻿using System.Reflection;
using Devon4Net.Infrastructure.Common.Options;
using Devon4Net.Infrastructure.Common.Options.MediatR;
using Devon4Net.Infrastructure.LiteDb.LiteDb;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Infrastructure.Log;
using Devon4Net.Infrastructure.MediatR.Data.Service;
using Devon4Net.Infrastructure.MediatR.Domain.Database;
using Devon4Net.Infrastructure.MediatR.Domain.Entities;
using Devon4Net.Infrastructure.MediatR.Domain.ServiceInterfaces;
using Devon4Net.Infrastructure.MediatR.Handler;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.Application.WebAPI.Configuration
{
    public static class MediatRConfiguration
    {
        public static void SetupMediatR(this IServiceCollection services, ref IConfiguration configuration)
        {
            var mediatROptions = services.GetTypedOptions<MediatROptions>(configuration, "MediatR");

            if (mediatROptions == null || !mediatROptions.EnableMediatR) return;
            ConfigureMediatRGenericDependencyInjection(ref services);
            SetupMediatRBackupLocalDatabase(ref services, ref mediatROptions);
        }

        private static void ConfigureMediatRGenericDependencyInjection(ref IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository<MediatRBackup>), typeof(Repository<MediatRBackup>));
            services.AddTransient(typeof(IMediatRBackupService), typeof(MediatRBackupService));
            services.AddTransient(typeof(IMediatRHandler), typeof(MediatRHandler));
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        private static void SetupMediatRBackupLocalDatabase(ref IServiceCollection services, ref MediatROptions mediatROptions)
        {
            Devon4NetLogger.Information("Please setup your database in order to have the RabbitMq messaging backup feature");
            if (mediatROptions.Backup == null || !mediatROptions.Backup.UseLocalBackup) return;
            Devon4NetLogger.Information("RabbitMq messaging backup feature is going to be used via LiteDb");

            services.AddSingleton<ILiteDbContext, MediatRBackupLiteDbContext>();
            services.AddTransient(typeof(IMediatRBackupLiteDbService), typeof(MediatRBackupLiteDbService));
        }
    }
}
