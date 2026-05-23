using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    // Servico responsavel pelas regras de negocio do Carrinho.
    // Ele faz a comunicacao entre a Controller e o Repository.
    // Tambem realiza a conversao entre DTOs e Entidades.
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IVariacaoRepository _variacaoRepository;
        private readonly IFreteService _freteService;

        // Recebe o repositorio por injecao de dependencia.
        public CarrinhoService(
            ICarrinhoRepository carrinhoRepository,
            IUsuarioRepository usuarioRepository,
            IVariacaoRepository variacaoRepository,
            IFreteService freteService)
        {
            _carrinhoRepository = carrinhoRepository;
            _usuarioRepository = usuarioRepository;
            _variacaoRepository = variacaoRepository;
            _freteService = freteService;
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

            // Caso nao encontre, retorna nulo.
            if (carrinho == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return MapearCarrinhoGetDto(carrinho);
        }

        // Busca o carrinho ativo de um usuário.
        // Se nao existir, cria um novo carrinho ativo para esse usuário.
        public async Task<CarrinhoGetDto> ObterOuCriarAtivoPorUsuarioAsync(int usuarioId)
        {
            await ValidarUsuarioAsync(usuarioId);

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
            await ValidarUsuarioAsync(carrinhoPostDto.UsuarioId);

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

            // Se nao existir, encerra o metodo.
            if (carrinho == null)
                return;

            var itensAtualizados = new List<ItemCarrinho>();

            foreach (var itemDto in carrinhoPutDto.Itens)
            {
                var variacao = await _variacaoRepository.ObterPorIdAsync(itemDto.VariacaoProdutoId);

                if (variacao == null)
                    throw new InvalidOperationException("Variacao do produto nao encontrada.");

                if (variacao.Produto == null)
                    throw new InvalidOperationException("Produto da variacao nao encontrado.");

                ValidarEstoque(variacao, itemDto.Quantidade);

                var itemAtualizado = carrinho.Itens.FirstOrDefault(item => item.Id == itemDto.Id) ?? new ItemCarrinho
                {
                    CarrinhoId = carrinho.Id
                };

                itemAtualizado.VariacaoProdutoId = itemDto.VariacaoProdutoId;
                itemAtualizado.VariacaoProduto = variacao;
                itemAtualizado.Quantidade = itemDto.Quantidade;
                itemAtualizado.PrecoUnitario = variacao.Produto.Preco;

                itensAtualizados.Add(itemAtualizado);
            }

            carrinho.Itens = itensAtualizados;

            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        // Remove um carrinho pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _carrinhoRepository.RemoverAsync(id);
        }


        // Valida se o usuário existe no sistema.
        private async Task ValidarUsuarioAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuario nao encontrado");
        }

        // Valida se a quantidade solicitada é válida
        // e se há estoque suficiente para a variação do produto.
        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variacao.");
        }
        // Converte a entidade Carrinho para CarrinhoGetDto.
        private CarrinhoGetDto MapearCarrinhoGetDto(Carrinho carrinho)
        {
            var itens = carrinho.Itens.Select(MapearItemCarrinhoGetDto).ToList();
            var subtotal = itens.Sum(item => item.Subtotal);
            var desconto = 0m;
            var frete = _freteService.CalcularFrete(subtotal - desconto);
            var totalGeral = subtotal - desconto + frete;

            return new CarrinhoGetDto
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                Ativo = carrinho.Ativo,
                TotalItens = itens.Sum(item => item.Quantidade),
                Subtotal = subtotal,
                Desconto = desconto,
                Frete = frete,
                ValorFrete = frete,
                TotalGeral = totalGeral,
                ValorTotalComFrete = totalGeral,
                Itens = itens
            };
        }

        // Converte a entidade ItemCarrinho para ItemCarrinhoGetDto,
        // incluindo informações do produto e variação.
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

        // Determina a URL da imagem do produto com base no nome do produto.
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

            if (nomeNormalizado.Contains("calca") || nomeNormalizado.Contains("calça") || nomeNormalizado.Contains("jeans"))
                return "/assets/products/calca-levis-clara.png";

            if (nomeNormalizado.Contains("corrente") || nomeNormalizado.Contains("ice"))
                return "/assets/products/corrente-ice.png";

            if (nomeNormalizado.Contains("adidas") || nomeNormalizado.Contains("tenis") || nomeNormalizado.Contains("tênis"))
                return "/assets/products/tenis2.png";

            return "/assets/products/camisa-over-black.png";
        }

        // Determina a marca do produto com base no nome do produto.
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
