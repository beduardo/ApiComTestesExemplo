using AutoMapper;

namespace Api
{
    public class PessoaMapper : Profile
    {
        public PessoaMapper()
        {
            CreateMap<PessoaModel, PessoaEntidade>();
            CreateMap<PessoaEntidade, PessoaModel>();
        }
    }
}