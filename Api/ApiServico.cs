using System.Collections.Generic;
using Api.Migrations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public interface IApiServico
    {
        Task<string> CriarPessoa(PessoaModel model);
        Task<IEnumerable<PessoaModel>> ListarPessoas();
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

        public async Task<string> CriarPessoa(PessoaModel model)
        {
            var entidade = mapper.Map<PessoaEntidade>(model);

            await contexto.Pessoas.AddAsync(entidade);
            await contexto.SaveChangesAsync();
            
            return entidade.Id.ToString();
        }

        public async Task<IEnumerable<PessoaModel>> ListarPessoas()
        {
            var entidades = await contexto.Pessoas.ToListAsync();
            var retorno = mapper.Map<IEnumerable<PessoaModel>>(entidades);
            return retorno;
        }
    }
}