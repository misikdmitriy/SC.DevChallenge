using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SC.DevChallenge.Api.MediatorRequests;
using SC.DevChallenge.Api.Middlewares;
using SC.DevChallenge.Core.Services;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Repositories;
using SC.DevChallenge.Db.Repositories.Contracts;
using Swashbuckle.AspNetCore.Swagger;

namespace SC.DevChallenge.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                var filePath = Path.Combine(AppContext.BaseDirectory, "SC.DevChallenge.Api.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(DbRepository<>))
                .As(typeof(IDbRepository<>));

            builder.RegisterType<DateTimeConverter>()
                .As<IDateTimeConverter>();

            builder.RegisterType<PriceModelParser>()
                .As<IPriceModelParser>();

            builder.Populate(services);

            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(AveragePriceRequest).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            // request handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()
            builder.RegisterAssemblyTypes(typeof(AveragePriceRequest).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandlerMiddleware();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            using (var context = app.ApplicationServices.GetService<AppDbContext>())
            {
                var conn = context.Database.GetDbConnection();

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var transaction = conn.BeginTransaction())
                {
                    ParseCsv(app.ApplicationServices.GetService<IPriceModelParser>(), conn, 
                        transaction);

                    transaction.Commit();
                }
            }
        }

        private static void ParseCsv(IPriceModelParser parser, DbConnection conn, DbTransaction transaction)
        {
            var command = File.ReadAllText(Path.Combine("Scripts", "script.sql"));

            using (var file = new StreamReader(Path.Combine("Input", "data.csv")))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    var separated = line.Split(';');

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = command
                            .Replace("'portfolio'", $"'{separated[0]}'")
                            .Replace("'owner'", $"'{separated[1]}'")
                            .Replace("'instrument'", $"'{separated[2]}'")
                            .Replace("'date'", $"'{separated[3]}'")
                            .Replace("'price'", separated[4]);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
