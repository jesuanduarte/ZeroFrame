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
            var itens = carrinho.Itens.Select(MapearItemCarrinhoGetDto).ToList();
            var subtotal = itens.Sum(item => item.Subtotal);
            var desconto = 0m;
            var frete = 0m;

            return new CarrinhoGetDto
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                Ativo = carrinho.Ativo,
                TotalItens = itens.Sum(item => item.Quantidade),
                Subtotal = subtotal,
                Desconto = desconto,
                Frete = frete,
                TotalGeral = subtotal - desconto + frete,
                Itens = itens
            };
        }

        private static ItemCarrinhoGetDto MapearItemCarrinhoGetDto(ItemCarrinho item)
        {
            var variacao = item.VariacaoProduto;
            var produto = variacao?.Produto;

            return new ItemCarrinhoGetDto
            {
                Id = item.Id,
                CarrinhoId = item.CarrinhoId,
                VariacaoProdutoId = item.VariacaoProdutoId,
                ProdutoId = produto?.Id ?? 0,
                NomeProduto = produto?.Nome ?? string.Empty,
                ImagemUrl = produto == null ? string.Empty : ObterImagemUrl(produto),
                CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,
                Marca = produto == null ? string.Empty : ObterMarca(produto),
                Origem = produto == null ? string.Empty : ObterOrigem(produto),
                Tamanho = variacao?.Tamanho ?? string.Empty,
                Cor = variacao?.Cor ?? string.Empty,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.Quantidade * item.PrecoUnitario
            };
        }

        private static string ObterImagemUrl(Produto produto)
        {
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("jordan") || nomeNormalizado.Contains("latte"))
                return "/assets/products/aj1-high-latte.png";

            if (nomeNormalizado.Contains("camisa") || nomeNormalizado.Contains("oversized"))
                return "/assets/products/camisa-over-black.png";

            if (nomeNormalizado.Contains("bermuda"))
                return "/assets/products/bermuda-jeans.jpg";

            if (nomeNormalizado.Contains("moletom") || nomeNormalizado.Contains("blusa"))
                return "/assets/products/blusa-moletom.jpg";

            if (nomeNormalizado.Contains("calca") || nomeNormalizado.Contains("cal�a") || nomeNormalizado.Contains("jeans"))
                return "/assets/products/calca-levis-clara.png";

            if (nomeNormalizado.Contains("corrente") || nomeNormalizado.Contains("ice"))
                return "/assets/products/corrente-ice.png";

            if (nomeNormalizado.Contains("adidas") || nomeNormalizado.Contains("tenis") || nomeNormalizado.Contains("t�nis"))
                return "/assets/products/tenis2.png";

            return "/assets/products/camisa-over-black.png";
        }

        private static string ObterMarca(Produto produto)
        {
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("nike") || nomeNormalizado.Contains("jordan"))
                return "Nike";

            if (nomeNormalizado.Contains("adidas"))
                return "Adidas";

            if (nomeNormalizado.Contains("levis") || nomeNormalizado.Contains("levi"))
                return "Levi's";

            return "Zero Frame";
        }

        private static string ObterOrigem(Produto produto)
        {
            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }
    }
}
