using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Text;
using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ApiTests
{
    public class ApiFixture : WebApplicationFactory<Startup>
    {
        public readonly HttpClient client;
        protected DbConnection conexaoSqlite { get; set; }
        protected ServiceProvider serviceProvider { get; set; }

        public ApiFixture()
        {
            client = CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        
        public StringContent ConvertToRawJson(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                UtilizarSQLiteComoBanco(services);
                serviceProvider = services.BuildServiceProvider();
            });
        }
        
        private void UtilizarSQLiteComoBanco(IServiceCollection services)
        {
            //Usar SQLite
            var descriptorContexto = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApiContext>));
            services.Remove(descriptorContexto);
            
            conexaoSqlite = new SqliteConnection("Filename=:memory:");
            conexaoSqlite.Open();
            services.AddDbContext<ApiContext>(options =>
            {
                options.UseSqlite(conexaoSqlite);
            });

            // var sp = services.BuildServiceProvider();
            // using (var scope = sp.CreateScope())
            // {
            //     var scopedServices = scope.ServiceProvider;
            //     var db = scopedServices.GetRequiredService<ApiContext>();
            //     db.Database.EnsureCreated();
            // }
        }
        
        public ApiContext CriarNovoContexto()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseSqlite(conexaoSqlite)
                .Options;
            return new ApiContext(options);
        }
        
    }
}