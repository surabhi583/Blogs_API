using System.Text;
using Blogs.Infrastructure.Authentication;

namespace Blogs.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
           
            configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddEndpointsApiExplorer();
            var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(new ConfigurationBuilder()
                        .AddJsonFile("SeriLogConfig.json")
                        .Build())
                        .Enrich.FromLogContext()
                        .CreateLogger();
            services.AddSerilog(logger);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
            });

            AddJWTTokenAuthentication(services, Configuration);

            services.AddSingleton<ITokenGenerator, TokenGenerator>();

            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IUserHandler, UserHandler>();
            services.AddScoped<IArticlesHandler, ArticlesHandler>();
            services.AddScoped<IProfilesHandler, ProfilesHandler>();

            services.AddAuthorization();

            services.AddDbContext<BlogsContext>(options => { options.UseSqlServer(Configuration.GetConnectionString("BlogConnString")); });

            services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "blogs", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "blogs v1"));
        }

        private void AddJWTTokenAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var JwtValidIssuer = configuration["AppSettings:ValidIssuer"];
            var JwtValidAudience = configuration["AppSettings:ValidAudience"];
            var JwtSecretkey = Encoding.ASCII.GetBytes(configuration["AppSettings:Secret"]);


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(JwtSecretkey),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ValidIssuer = JwtValidIssuer,
                ValidAudience = JwtValidAudience
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
                jwt.Events = new JwtBearerEvents { OnMessageReceived = CustomOnMessageReceivedHandler.OnMessageReceived };
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });
        }
    }
}
