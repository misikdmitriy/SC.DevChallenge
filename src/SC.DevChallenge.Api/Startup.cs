using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Extensions.Logging;
using SC.DevChallenge.Api.MediatorRequests;
using SC.DevChallenge.Api.Middlewares;
using SC.DevChallenge.Core.Services;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories;
using SC.DevChallenge.Db.Factories.Contracts;
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

            var builder = RegisterDependencies();

            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public static ContainerBuilder RegisterDependencies()
        {
	        var builder = new ContainerBuilder();

	        builder.RegisterGeneric(typeof(DbRepository<>))
		        .As(typeof(IDbRepository<>));

	        builder.RegisterType<DateTimeConverter>()
		        .As<IDateTimeConverter>();

	        builder.RegisterType<AppDbContextFactory>()
		        .As<IDbContextFactory<AppDbContext>>();

	        builder.RegisterType<ContentFactory>()
		        .As<IContentFactory>();

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

			return builder;
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

            var requestOpt = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> {CultureInfo.InvariantCulture},
                SupportedUICultures = new List<CultureInfo> {CultureInfo.InvariantCulture}
            };

            app.UseRequestLocalization(requestOpt);

            var contentFactory = app.ApplicationServices.GetService<IContentFactory>();
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            if (contentFactory.IsUpdateRequired())
            {
	            logger.LogInformation("DB update started");
				contentFactory.ParseContentFromCsv(Path.Combine("Input", "data.csv"));
			}
            else
            {
	            logger.LogInformation("Skip DB update");
			}
		}
    }
}
