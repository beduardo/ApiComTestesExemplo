using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("pessoas")]
    public class PessoasController : Controller
    {
        private readonly IApiServico servico;

        public PessoasController(IApiServico servico)
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

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var pessoas = servico.ListarPessoas();

                return Ok(new RespostaApi<IEnumerable<PessoaModel>>
                {
                    dados = pessoas
                });
            }
            catch (Exception e)
            {
                return Ok(new RespostaApi<IEnumerable<PessoaModel>>
                {
                    erro = true,
                    mensagem = e.Message
                });
            }
        }
    }
}