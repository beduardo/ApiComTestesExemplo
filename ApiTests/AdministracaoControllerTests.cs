using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests
{
    [TestClass]
    public class AdministracaoControllerTests
    {
        private const string urlroot = "/administracao";
        private const string urlrootadmin = "/administracao/restricted";
        
        [TestMethod]
        public async Task Estatisticas_AcessoAnonimo_RecebeNaoAuthorizado()
        {
            var fixture = new ApiFixture();
            var resp = await fixture.client.GetAsync(urlroot);

            Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
        }    
        
        [TestMethod]
        public async Task Estatisticas_AcessoLogado_RecebeOk()
        {
            var fixture = new ApiFixture();

            var tokenobj = fixture.CreateTokenObject("user0001", "mymail@here.com"); 
            fixture.client.SetFakeBearerToken((object) tokenobj);
            
            var resp = await fixture.client.GetAsync(urlroot);

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }   
        
        [TestMethod]
        public async Task Estatisticas_Restricted_AcessoLogadoAdmin_RecebeOk()
        {
            var fixture = new ApiFixture();

            var tokenobj = fixture.CreateTokenObject("user0001", "mymail@here.com", new [] { "admin"}); 
            fixture.client.SetFakeBearerToken((object) tokenobj);
            
            var resp = await fixture.client.GetAsync(urlrootadmin);

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }
        
        [TestMethod]
        public async Task Estatisticas_Restricted_AcessoLogadoNormal_RecebeNaoPermitido()
        {
            var fixture = new ApiFixture();

            var tokenobj = fixture.CreateTokenObject("user0001", "mymail@here.com"); 
            fixture.client.SetFakeBearerToken((object) tokenobj);
            
            var resp = await fixture.client.GetAsync(urlrootadmin);

            Assert.AreEqual(HttpStatusCode.Forbidden, resp.StatusCode);
        }
        
        [TestMethod]
        public async Task Estatisticas_Restricted_AcessoLogadoOutroPerfil_RecebeNaoPermitido()
        {
            var fixture = new ApiFixture();

            var tokenobj = fixture.CreateTokenObject("user0001", "mymail@here.com", new []{"usuario"}); 
            fixture.client.SetFakeBearerToken((object) tokenobj);
            
            var resp = await fixture.client.GetAsync(urlrootadmin);

            Assert.AreEqual(HttpStatusCode.Forbidden, resp.StatusCode);
        }
    }
}