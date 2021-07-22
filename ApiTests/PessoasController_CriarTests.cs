using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Api;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NuGet.Frameworks;

namespace ApiTests
{
    [TestClass]
    public class PessoasController_CriarTests
    {
        // POST
        const string url = "/pessoas";
        
        [TestMethod]
        public async Task Criar_RespondeCreated()
        {
            var fixture = new ApiFixture();
            var model = fixture.fakerPessoaModel.Generate();

            var content = fixture.ConvertToRawJson(model);
            var resp = await fixture.client.PostAsync(url, content);

            Assert.AreEqual(HttpStatusCode.Created, resp.StatusCode);
        }

        [TestMethod]
        public async Task Criar_InserePessoaNoBanco()
        {
            var fixture = new ApiFixture();
            var model = fixture.fakerPessoaModel.Generate();

            var content = fixture.ConvertToRawJson(model);
            await fixture.client.PostAsync(url, content);

            var contexto = fixture.CriarNovoContexto();

            var pessoa = contexto.Pessoas.FirstOrDefault();
            
            Assert.IsNotNull(pessoa);
            Assert.AreEqual(model.Nome, pessoa.Nome);
        }

        [TestMethod]
        public async Task Criar_InserePessoaNoBancoComNovoId()
        {
            var fixture = new ApiFixture();
            var model = fixture.fakerPessoaModel.Generate();

            var content = fixture.ConvertToRawJson(model);
            await fixture.client.PostAsync(url, content);

            var contexto = fixture.CriarNovoContexto();
            var pessoa = contexto.Pessoas.FirstOrDefault();
            
            Assert.IsNotNull(pessoa);
            Assert.AreNotEqual(Guid.Empty, pessoa.Id);
        }
        
        [TestMethod]
        public async Task Criar_RetornaPessoaComId()
        {
            var fixture = new ApiFixture();
            var model = fixture.fakerPessoaModel.Generate();

            var content = fixture.ConvertToRawJson(model);
            var resp = await fixture.client.PostAsync(url, content);
            
            string responseJson = await resp.Content.ReadAsStringAsync();
            var resposta = JsonConvert.DeserializeObject<PessoaModel>(responseJson);
            
            var contexto = fixture.CriarNovoContexto();
            var pessoa = contexto.Pessoas.FirstOrDefault();
            
            //Guard
            Assert.IsNotNull(pessoa);
            Assert.AreEqual(pessoa.Id.ToString(), resposta.Id);
        }

        [TestMethod]
        public void Criar_ServiceGeraException_RetornaMensagem()
        {
            var fixture = new ApiFixture();
            var model = fixture.fakerPessoaModel.Generate();

            var mockServico = new Mock<IApiServico>();
            var controller = new PessoasController(mockServico.Object);

            mockServico.Setup(m => m.CriarPessoa(It.IsAny<PessoaModel>()))
                .Throws(new Exception("Mensagem de Erro"));

            var respAction = controller.Criar(model);
            var CreateResult = (OkObjectResult) respAction;
            var resposta = CreateResult.Value as RespostaApi<PessoaModel>;
            
            Assert.IsNull(resposta.dados);
            Assert.AreEqual(true, resposta.erro);
            Assert.AreEqual("Mensagem de Erro", resposta.mensagem);

        }
    }
}