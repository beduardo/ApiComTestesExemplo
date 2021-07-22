using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiTests
{
    [TestClass]
    public class PessoasController_ListarTests
    {
        // GET
        const string url = "/pessoas";

        [TestMethod]
        public async Task Lista_RespondeOKObjectResult()
        {
            var fixture = new ApiFixture();
            var resp = await fixture.client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }
        
        [TestMethod]
        public async Task Lista_RespondeResultadoVazioCorreto()
        {
            var fixture = new ApiFixture();
            
            var resp = await fixture.client.GetAsync(url);
            
            string responseJson = await resp.Content.ReadAsStringAsync();
            var resposta = JsonConvert.DeserializeObject<RespostaApi<IEnumerable<PessoaModel>>>(responseJson);

            var respostaEsperada = new RespostaApi<IEnumerable<PessoaModel>>
            {
                dados = new PessoaModel[] { }
            };

            resposta.Should().BeEquivalentTo(respostaEsperada);
        }
        
        [TestMethod]
        public async Task Lista_RespondeResultadoCorreto()
        {
            var fixture = new ApiFixture();

            var entidadesTeste = fixture.fakerPessoaEntidade.Generate(15);
            var contextoPre = fixture.CriarNovoContexto();
            
            await contextoPre.Pessoas.AddRangeAsync(entidadesTeste);
            await contextoPre.SaveChangesAsync();
            
            var resp = await fixture.client.GetAsync(url);
            
            string responseJson = await resp.Content.ReadAsStringAsync();
            var resposta = JsonConvert.DeserializeObject<RespostaApi<IEnumerable<PessoaModel>>>(responseJson);

            var respostaEsperada = new RespostaApi<IEnumerable<PessoaModel>>
            {
                dados = fixture.mapper.Map<IEnumerable<PessoaModel>>(entidadesTeste)
            };
            
            resposta.Should().BeEquivalentTo(respostaEsperada);
        }
    }
}