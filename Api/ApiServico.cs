using Api.Migrations;

namespace Api
{
    public interface IApiServico
    {
        string CriarPessoa(PessoaModel model);
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
    }
}