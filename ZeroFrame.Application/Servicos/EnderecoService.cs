using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do Endereço.
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;

        // Recebe o repositório por injeção de dependência.
        public EnderecoService(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        // Busca todos os endereços cadastrados.
        public async Task<List<EnderecoGetDto>> ObterTodosAsync()
        {
            var enderecos = await _enderecoRepository.ObterTodosAsync();

            // Converte a lista de entidades para lista de DTOs.
            return enderecos.Select(MapearEnderecoGetDto).ToList();
        }

        // Busca um endereço pelo Id.
        public async Task<EnderecoGetDto?> ObterPorIdAsync(int id)
        {
            var endereco = await _enderecoRepository.ObterPorIdAsync(id);

            // Caso não encontre, retorna nulo.
            if (endereco == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return MapearEnderecoGetDto(endereco);
        }

        // Cria um novo endereço.
        public async Task<EnderecoGetDto> CriarAsync(EnderecoPostDto enderecoPostDto)
        {
            // Converte o DTO recebido em entidade.
            var endereco = new Endereco
            {
                Rua = enderecoPostDto.Rua,
                Numero = enderecoPostDto.Numero,
                Cidade = enderecoPostDto.Cidade,
                Estado = enderecoPostDto.Estado,
                CEP = enderecoPostDto.Cep,
                UsuarioId = enderecoPostDto.UsuarioId,
                Ativo = true
            };

            await _enderecoRepository.AdicionarAsync(endereco);

            // Retorna os dados cadastrados em formato DTO.
            return MapearEnderecoGetDto(endereco);
        }

        // Atualiza um endereço existente.
        public async Task AtualizarAsync(EnderecoPutDto enderecoPutDto)
        {
            var endereco = await _enderecoRepository.ObterPorIdAsync(enderecoPutDto.Id);

            // Se não existir, encerra o método.
            if (endereco == null)
                return;

            // Atualiza os dados do endereço.
            endereco.Rua = enderecoPutDto.Rua;
            endereco.Numero = enderecoPutDto.Numero;
            endereco.Cidade = enderecoPutDto.Cidade;
            endereco.Estado = enderecoPutDto.Estado;
            endereco.CEP = enderecoPutDto.Cep;
            endereco.Ativo = enderecoPutDto.Ativo;
            endereco.UsuarioId = enderecoPutDto.UsuarioId;

            await _enderecoRepository.AtualizarAsync(endereco);
        }

        // Remove um endereço pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _enderecoRepository.RemoverAsync(id);
        }

        // Converte a entidade Endereco para EnderecoGetDto.
        private static EnderecoGetDto MapearEnderecoGetDto(Endereco endereco)
        {
            return new EnderecoGetDto
            {
                Id = endereco.Id,
                Rua = endereco.Rua,
                Numero = endereco.Numero,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.CEP,
                Ativo = endereco.Ativo,
                UsuarioId = endereco.UsuarioId
            };
        }
    }
}
