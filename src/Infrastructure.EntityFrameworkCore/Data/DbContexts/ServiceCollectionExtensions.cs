﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Data.DbContexts
{
    /// <summary>
    /// Extensions to add AddDbContextFactory
    /// https://github.com/vany0114/EF.DbContextFactory/blob/master/src/Extensions/EFCore.DbContextFactory/Extensions/ServiceCollectionExtensions.cs
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">The DbContext options.</param>
        public static void AddDbContextFactory<TDataContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TDataContext : DbContext
            => services.AddDbContextFactory<TDataContext>((_, builder) => options.Invoke(builder));

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Service provider and DbContext options.</param>
        public static void AddDbContextFactory<TDataContext>(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction) where TDataContext : DbContext
        {
            AddCoreServices<TDataContext>(services, optionsAction, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<DbContextOptions<TDataContext>>();

            services.AddScoped(_ => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options)!);
        }

        private static void AddCoreServices<TContextImplementation>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : DbContext
        {
            serviceCollection
                .AddMemoryCache()
                .AddLogging();

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContextImplementation>),
                    p => DbContextOptionsFactory<TContextImplementation>(p, optionsAction),
                    optionsLifetime));

            serviceCollection.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContextImplementation>>(),
                    optionsLifetime));
        }

        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>(
                new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);
            optionsAction.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}