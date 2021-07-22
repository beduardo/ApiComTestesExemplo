using System.Collections.Generic;
using Api.Migrations;
using System.Linq;
namespace Api
{
    public interface IApiServico
    {
        string CriarPessoa(PessoaModel model);
        IEnumerable<PessoaModel> ListarPessoas();
    }

    public class ApiServico : IApiServico
    {
        private readonly ApiContext contexto;

        public ApiServico(ApiContext contexto)
        {
            this.contexto = contexto;
        }

        public string CriarPessoa(PessoaModel model)
        {
            var entidade = new PessoaEntidade
            {
                Nome = model.Nome
            };

            contexto.Pessoas.Add(entidade);
            contexto.SaveChanges();
            
            return entidade.Id.ToString();
        }

        public IEnumerable<PessoaModel> ListarPessoas()
        {
            var retorno = from p in contexto.Pessoas
                select new PessoaModel
                {
                    Id = p.Id.ToString(),
                    Nome = p.Nome
                };

            return retorno.ToList();
        }
    }
}