using System;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using WebMotions.Fake.Authentication.JwtBearer;

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
                // UtilizarTokensJWTFakes(services);
                
                serviceProvider = services.BuildServiceProvider();
            });
            
            
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer(opt =>
                {
                    opt.Audience = "minha_aplicacao";
                    opt.Authority = "https://teste.com/minha_aplicacao";
                });
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
        }

        private void UtilizarTokensJWTFakes(IServiceCollection services)
        {
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var config = new OpenIdConnectConfiguration()
                {
                    Issuer = MockJwtTokens.Issuer
                };

                config.SigningKeys.Add(MockJwtTokens.SecurityKey);
                options.Configuration = config;
            });
        }
        
        public ApiContext CriarNovoContexto()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseSqlite(conexaoSqlite)
                .Options;
            return new ApiContext(options);
        }
        
        public dynamic CreateTokenObject(string subjectId, string email, string[] role = null)
        {
            dynamic tokenobj = new ExpandoObject();

            tokenobj.iss = "https://teste.com/minha_aplicacao";
            tokenobj.aud = "minha_aplicacao";
            if (role != null && role.Any())
            {
                tokenobj.role = role;
            }
            tokenobj.auth_time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            tokenobj.iat = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            tokenobj.exp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            tokenobj.sub = subjectId;
            tokenobj.email = email;
            tokenobj.sign_in_provider = "password";

            return tokenobj;
        }

        protected async Task<TResponse> BuscaRespostaTipada<TResponse>(HttpResponseMessage httpResponseMessage,
            HttpStatusCode statusCodeEsperado)
        {
            string respostaConteudo = await httpResponseMessage.Content.ReadAsStringAsync();
            httpResponseMessage.StatusCode.Should()
                .Be(statusCodeEsperado, $"{httpResponseMessage.StatusCode} - {respostaConteudo}");

            TResponse respostaTipada = default(TResponse);
            try
            {
                respostaTipada = JsonConvert.DeserializeObject<TResponse>(respostaConteudo);
                respostaTipada.Should().NotBeNull($"Deveria não ser nulo - Resposta para serializar: {respostaConteudo}");
            }
            catch (Exception ex)
            {
                ex.Should().BeNull($"Exception - Respose para Serializar: {respostaConteudo}");
            }

            return respostaTipada;
        }
        
    }
}