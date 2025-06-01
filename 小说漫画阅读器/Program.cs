
using System.Reflection;
using 小说漫画阅读器.Controllers;

namespace 小说漫画阅读器
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<CancellationTokenPool>();
            builder.Services.AddSingleton<CancelController>();
            builder.Services.AddSingleton<MangaTagDic>();
            builder.Services.AddScoped<TestController>();
            builder.Services.AddScoped<ILoginController,LoginController>();

            builder.Services.AddHttpClient("MangaClient", client =>
            {
                client.BaseAddress = new Uri("https://api.mangadex.org");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");
            });

 


            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(b =>
                {
                    b.WithOrigins(new string[] { "http://localhost:5173" })
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            //启用内存缓存必须在UseCors以后，MapControllers之前
            app.UseResponseCaching();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            


            app.MapControllers();

            app.Run();
        }
    }
}
