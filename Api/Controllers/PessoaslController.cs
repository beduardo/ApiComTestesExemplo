using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("pessoas")]
    public class PessoaslController : Controller
    {
        private readonly IApiServico servico;
        public PessoaslController(IApiServico servico)
        {
            this.servico = servico;
        }
        
        [HttpPost]
        public IActionResult Criar([FromBody] PessoaModel model)
        {
            try
            {
                var id = servico.CriarPessoa(model);

                model.Id = id;
                return Created("/pessoas", model);
            }
            catch (Exception e)
            {
                return Ok(new RespostaApi<PessoaModel>
                {
                    erro = true,
                    mensagem = e.Message
                });
            }
        }
    }
}