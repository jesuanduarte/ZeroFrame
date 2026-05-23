using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do Endereço.
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        // Recebe o repositório por injeção de dependência.
        public EnderecoService(IEnderecoRepository enderecoRepository, IUsuarioRepository usuarioRepository)
        {
            _enderecoRepository = enderecoRepository;
            _usuarioRepository = usuarioRepository;
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


        public async Task<EnderecoGetDto?> ObterPorUsuarioIdAsync(int usuarioId)
        {
            var endereco = await _enderecoRepository.ObterPorUsuarioIdAsync(usuarioId);

            if (endereco == null)
                return null;

            return MapearEnderecoGetDto(endereco);
        }

        public async Task<List<EnderecoGetDto>> ObterTodosPorUsuarioIdAsync(int usuarioId)
        {
            await ValidarUsuarioAsync(usuarioId);

            var enderecos = await _enderecoRepository.ObterTodosPorUsuarioIdAsync(usuarioId);
            return enderecos.Select(MapearEnderecoGetDto).ToList();
        }
        // Cria um novo endereço.
        public async Task<EnderecoGetDto> CriarAsync(EnderecoPostDto enderecoPostDto)
        {
            await ValidarUsuarioAsync(enderecoPostDto.UsuarioId);
            await ValidarLimiteEnderecosAsync(enderecoPostDto.UsuarioId);

            // Converte o DTO recebido em entidade.
            var endereco = new Endereco
            {
                Rua = enderecoPostDto.Rua,
                Numero = enderecoPostDto.Numero,
                Bairro = enderecoPostDto.Bairro,
                Cidade = enderecoPostDto.Cidade,
                Estado = enderecoPostDto.Estado.Trim().ToUpperInvariant(),
                CEP = enderecoPostDto.Cep,
                Telefone = enderecoPostDto.Telefone,
                Complemento = enderecoPostDto.Complemento,
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
                throw new KeyNotFoundException("Endereco nao encontrado");

            await ValidarUsuarioAsync(enderecoPutDto.UsuarioId);

            // Atualiza os dados do endereço.
            endereco.Rua = enderecoPutDto.Rua;
            endereco.Numero = enderecoPutDto.Numero;
            endereco.Bairro = enderecoPutDto.Bairro;
            endereco.Cidade = enderecoPutDto.Cidade;
            endereco.Estado = enderecoPutDto.Estado.Trim().ToUpperInvariant();
            endereco.CEP = enderecoPutDto.Cep;
            endereco.Telefone = enderecoPutDto.Telefone;
            endereco.Complemento = enderecoPutDto.Complemento;
            if (enderecoPutDto.Ativo.HasValue)
                endereco.Ativo = enderecoPutDto.Ativo.Value;
            endereco.UsuarioId = enderecoPutDto.UsuarioId;

            await _enderecoRepository.AtualizarAsync(endereco);
        }

        // Remove um endereço pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _enderecoRepository.RemoverAsync(id);
        }

        // Método para validar se o usuário existe antes de criar ou atualizar um endereço.
        private async Task ValidarUsuarioAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuario nao encontrado");
        }

        // O limite vale apenas para criacao; atualizar endereco existente nao incrementa a contagem.
        private async Task ValidarLimiteEnderecosAsync(int usuarioId)
        {
            var totalEnderecos = await _enderecoRepository.CountByUsuarioIdAsync(usuarioId);

            if (totalEnderecos >= 3)
                throw new InvalidOperationException("O usuário já possui o limite máximo de 3 endereços cadastrados.");
        }

        // Converte a entidade Endereco para EnderecoGetDto.
        private static EnderecoGetDto MapearEnderecoGetDto(Endereco endereco)
        {
            return new EnderecoGetDto
            {
                Id = endereco.Id,
                Rua = endereco.Rua,
                Numero = endereco.Numero,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.CEP,
                Telefone = endereco.Telefone,
                Complemento = endereco.Complemento,
                Ativo = endereco.Ativo,
                UsuarioId = endereco.UsuarioId
            };
        }
    }
}
