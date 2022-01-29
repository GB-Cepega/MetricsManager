﻿using AutoMapper;
using FluentMigrator.Runner;
using MetricsAgent.DAL;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Repository;
using MetricsAgent.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace MetricsAgent
{ 
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            private IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();

                services.AddSingleton<ISqlSettingsProvider, SqlSettingsProvider>();
                services.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();
                services.AddSingleton<IDotNetMetricsRepository, DotNetMetricsRepository>();
                services.AddSingleton<IHddMetricsRepository, HddMetricsRepository>();
                services.AddSingleton<IRamMetricsRepository, RamMetricsRepository>();
                services.AddSingleton<INetworkMetricsRepository, NetworkMetricsRepository>();

                var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
                var mapper = mapperConfiguration.CreateMapper();
                services.AddSingleton(mapper);

                services.AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb
                        .AddSQLite()
                        .WithGlobalConnectionString(new SqlSettingsProvider().GetConnectionString())
                        .ScanIn(typeof(Startup).Assembly).For.Migrations()
                    ).AddLogging(lb => lb
                        .AddFluentMigratorConsole());

                services.AddSingleton<IJobFactory, SingletonJobFactory>();
                services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                services.AddSingleton<CpuMetricJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(CpuMetricJob),
                    cronExpression: "0/5 * * * * ?"));
                services.AddSingleton<RamMetricJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(RamMetricJob),
                    cronExpression: "0/5 * * * * ?"));
                services.AddSingleton<HddMetricJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(HddMetricJob),
                    cronExpression: "0/5 * * * * ?"));
                services.AddSingleton<DotNetMetricJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(DotNetMetricJob),
                    cronExpression: "0/5 * * * * ?"));
                services.AddSingleton<NetworkMetricJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(NetworkMetricJob),
                    cronExpression: "0/5 * * * * ?"));
                services.AddHostedService<QuartzHostedService>();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "API ñåðâèñà àãåíòà ñáîðà ìåòðèê",
                        Description = "Ñòðàíèöà äëÿ òåñòèðîâàíèÿ ðàáîòû API",
                        TermsOfService = new Uri("https://coderda.com"),
                        Contact = new OpenApiContact
                        {
                            Name = "Kiverin",
                            Email = string.Empty,
                            Url = new Uri("https://coderda.com"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Free license",
                            Url = new Uri("https://coderda.com"),
                        }
                    });
                    var xmlFile =
                        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                migrationRunner.MigrateUp();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ñåðâèñà àãåíòà ñáîðà ìåòðèê");
                    c.RoutePrefix = string.Empty;
                });
            }

        }
    
}
