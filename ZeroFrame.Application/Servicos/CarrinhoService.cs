using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do Carrinho.
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ICarrinhoRepository _carrinhoRepository;

        // Recebe o repositório por injeção de dependência.
        public CarrinhoService(ICarrinhoRepository carrinhoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
        }

        // Busca todos os carrinhos cadastrados.
        public async Task<List<CarrinhoGetDto>> ObterTodosAsync()
        {
            var carrinhos = await _carrinhoRepository.ObterTodosAsync();

            // Converte a lista de entidades para lista de DTOs.
            return carrinhos.Select(MapearCarrinhoGetDto).ToList();
        }

        // Busca um carrinho pelo Id.
        public async Task<CarrinhoGetDto?> ObterPorIdAsync(int id)
        {
            var carrinho = await _carrinhoRepository.ObterPorIdAsync(id);

            // Caso não encontre, retorna nulo.
            if (carrinho == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return MapearCarrinhoGetDto(carrinho);
        }


        public async Task<CarrinhoGetDto> ObterOuCriarAtivoPorUsuarioAsync(int usuarioId)
        {
            var carrinho = await _carrinhoRepository.ObterAtivoPorUsuarioAsync(usuarioId);

            if (carrinho == null)
            {
                carrinho = new Carrinho
                {
                    UsuarioId = usuarioId,
                    Ativo = true
                };

                await _carrinhoRepository.AdicionarAsync(carrinho);
            }

            return MapearCarrinhoGetDto(carrinho);
        }
        // Cria um novo carrinho.
        public async Task<CarrinhoGetDto> CriarAsync(CarrinhoPostDto carrinhoPostDto)
        {
            // Converte o DTO recebido em entidade.
            var carrinho = new Carrinho
            {
                UsuarioId = carrinhoPostDto.UsuarioId,
                Ativo = true
            };

            await _carrinhoRepository.AdicionarAsync(carrinho);

            // Retorna os dados cadastrados em formato DTO.
            return MapearCarrinhoGetDto(carrinho);
        }

        // Atualiza os itens de um carrinho existente.
        public async Task AtualizarAsync(CarrinhoPutDto carrinhoPutDto)
        {
            var carrinho = await _carrinhoRepository.ObterPorIdAsync(carrinhoPutDto.Id);

            // Se não existir, encerra o método.
            if (carrinho == null)
                return;

            // Atualiza a lista de itens do carrinho.
            carrinho.Itens = carrinhoPutDto.Itens.Select(item => new ItemCarrinho
            {
                Id = item.Id,
                CarrinhoId = carrinho.Id,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade
            }).ToList();

            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        // Remove um carrinho pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _carrinhoRepository.RemoverAsync(id);
        }

        // Converte a entidade Carrinho para CarrinhoGetDto.
        private static CarrinhoGetDto MapearCarrinhoGetDto(Carrinho carrinho)
        {
            return new CarrinhoGetDto
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                Ativo = carrinho.Ativo,
                Itens = carrinho.Itens.Select(item => new ItemCarrinhoGetDto
                {
                    Id = item.Id,
                    CarrinhoId = item.CarrinhoId,
                    VariacaoProdutoId = item.VariacaoProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };
        }
    }
}
