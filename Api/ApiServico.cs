using System.Collections.Generic;
using Api.Migrations;
using System.Linq;
using AutoMapper;

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
        private readonly IMapper mapper;

        public ApiServico(ApiContext contexto, IMapper mapper)
        {
            this.contexto = contexto;
            this.mapper = mapper;
        }

        public string CriarPessoa(PessoaModel model)
        {
            var entidade = mapper.Map<PessoaEntidade>(model);

            contexto.Pessoas.Add(entidade);
            contexto.SaveChanges();
            
            return entidade.Id.ToString();
        }

        public IEnumerable<PessoaModel> ListarPessoas()
        {
            var entidades = contexto.Pessoas.ToList();
            var retorno = mapper.Map<IEnumerable<PessoaModel>>(entidades);
            return retorno;
        }
    }
}