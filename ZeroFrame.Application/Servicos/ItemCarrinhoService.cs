using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.domain.entidades;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    public class ItemCarrinhoService : IItemCarrinhoService
    {
        private readonly IItemCarrinhoRepository _itemCarrinhoRepository;
        private readonly IVariacaoRepository _variacaoRepository;

        public ItemCarrinhoService(
            IItemCarrinhoRepository itemCarrinhoRepository,
            IVariacaoRepository variacaoRepository)
        {
            _itemCarrinhoRepository = itemCarrinhoRepository;
            _variacaoRepository = variacaoRepository;
        }

        // Busca todos os itens de carrinho cadastrados no banco de dados.
        public async Task<List<ItemCarrinhoGetDto>> ObterTodosAsync()
        {
            var itens = await _itemCarrinhoRepository.ObterTodosAsync();
            return itens.Select(MapearItemCarrinhoGetDto).ToList();
        }

        // Busca um item de carrinho específico pelo Id.
        public async Task<ItemCarrinhoGetDto?> ObterPorIdAsync(int id)
        {
            var item = await _itemCarrinhoRepository.ObterPorIdAsync(id);

            if (item == null)
                return null;

            return MapearItemCarrinhoGetDto(item);
        }

        // Busca todos os itens que pertencem a um carrinho específico.
        public async Task<List<ItemCarrinhoGetDto>> ObterPorCarrinhoAsync(int carrinho)
        {
            var itens = await _itemCarrinhoRepository.ObterPorCarrinhoAsync(carrinho);
            return itens.Select(MapearItemCarrinhoGetDto).ToList();
        }

        // Cria um novo item no carrinho ou atualiza a quantidade caso o item já exista.
        public async Task<ItemCarrinhoGetDto> CriarAsync(ItemCarrinhoPostDto itemCarrinhoPostDto)
        {
            var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinhoPostDto.VariacaoProdutoId);

            if (variacao == null)
                throw new InvalidOperationException("Variacao do produto nao encontrada.");

            if (variacao.Produto == null)
                throw new InvalidOperationException("Produto da variacao nao encontrado.");

            var itemExistente = await _itemCarrinhoRepository.ObterPorCarrinhoEVariacaoAsync(
                itemCarrinhoPostDto.CarrinhoId,
                itemCarrinhoPostDto.VariacaoProdutoId);

            var quantidadeFinal = itemCarrinhoPostDto.Quantidade + (itemExistente?.Quantidade ?? 0);
            ValidarEstoque(variacao, quantidadeFinal);

            if (itemExistente != null)
            {
                itemExistente.Quantidade = quantidadeFinal;
                itemExistente.PrecoUnitario = variacao.Produto.Preco;

                await _itemCarrinhoRepository.AtualizarAsync(itemExistente);
                return MapearItemCarrinhoGetDto(itemExistente);
            }

            var item = new ItemCarrinho
            {
                CarrinhoId = itemCarrinhoPostDto.CarrinhoId,
                VariacaoProdutoId = itemCarrinhoPostDto.VariacaoProdutoId,
                Quantidade = itemCarrinhoPostDto.Quantidade,
                PrecoUnitario = variacao.Produto.Preco
            };

            await _itemCarrinhoRepository.AdicionarAsync(item);
            item.VariacaoProduto = variacao;
            return MapearItemCarrinhoGetDto(item);
        }

        // Atualiza os dados de um item já existente no carrinho.
        public async Task AtualizarAsync(ItemCarrinhoPutDto itemCarrinhoPutDto)
        {
            var item = await _itemCarrinhoRepository.ObterPorIdAsync(itemCarrinhoPutDto.Id);

            if (item == null)
                return;

            var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinhoPutDto.VariacaoProdutoId);

            if (variacao == null)
                throw new InvalidOperationException("Variacao do produto nao encontrada.");

            if (variacao.Produto == null)
                throw new InvalidOperationException("Produto da variacao nao encontrado.");

            ValidarEstoque(variacao, itemCarrinhoPutDto.Quantidade);

            item.VariacaoProdutoId = itemCarrinhoPutDto.VariacaoProdutoId;
            item.Quantidade = itemCarrinhoPutDto.Quantidade;
            item.PrecoUnitario = variacao.Produto.Preco;

            await _itemCarrinhoRepository.AtualizarAsync(item);
        }

        // Remove um item do carrinho com base no Id 
        public async Task RemoverAsync(int id)
        {
            await _itemCarrinhoRepository.RemoverAsync(id);
        }

        // faz a validação para garantir que a quantidade solicitada não seja negativa e que haja estoque suficiente para a variação do produto.
        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variacao.");
        }


        // Converte a entidade ItemCarrinho para ItemCarrinhoGetDto.
        private static ItemCarrinhoGetDto MapearItemCarrinhoGetDto(ItemCarrinho item)
        {
            // Pega a variação do produto vinculada ao item do carrinho.
            var variacao = item.VariacaoProduto;

            // Pega o produto vinculado à variação.
            var produto = variacao?.Produto;

            // Retorna um DTO com os dados formatados para a API/front-end.
            return new ItemCarrinhoGetDto
            {
                Id = item.Id,
                CarrinhoId = item.CarrinhoId,
                VariacaoProdutoId = item.VariacaoProdutoId,

                // Se o produto existir, usa o Id dele. Caso contrário, retorna 0.
                ProdutoId = produto?.Id ?? 0,

                // Se o produto existir, usa o nome dele. Caso contrário, retorna texto vazio.
                NomeProduto = produto?.Nome ?? string.Empty,

                // Define a imagem do produto com base no nome.
                ImagemUrl = produto == null ? string.Empty : ObterImagemUrl(produto),

                // Pega o nome da categoria, caso exista.
                CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,

                // Define a marca do produto com base no nome.
                Marca = produto == null ? string.Empty : ObterMarca(produto),

                // Define se o produto é original da loja ou multimarcas.
                Origem = produto == null ? string.Empty : ObterOrigem(produto),

                // Pega o tamanho da variação, caso exista.
                Tamanho = variacao?.Tamanho ?? string.Empty,

                // Pega a cor da variação, caso exista.
                Cor = variacao?.Cor ?? string.Empty,

                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,

                // Calcula o subtotal do item: quantidade vezes preço unitário.
                Subtotal = item.Quantidade * item.PrecoUnitario
            };
        }

        // Define qual imagem será usada para o produto com base no nome dele.
        private static string ObterImagemUrl(Produto produto)
        {
            // Normaliza o nome para minúsculo para facilitar a comparação.
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            // Se o nome tiver Jordan ou Latte, retorna imagem do Air Jordan.
            if (nomeNormalizado.Contains("jordan") || nomeNormalizado.Contains("latte"))
                return "/assets/products/aj1-high-latte.png";

            // Se o nome tiver Camisa ou Oversized, retorna imagem da camisa.
            if (nomeNormalizado.Contains("camisa") || nomeNormalizado.Contains("oversized"))
                return "/assets/products/camisa-over-black.png";

            // Se o nome tiver Bermuda, retorna imagem da bermuda.
            if (nomeNormalizado.Contains("bermuda"))
                return "/assets/products/bermuda-jeans.jpg";

            // Se o nome tiver Moletom ou Blusa, retorna imagem do moletom.
            if (nomeNormalizado.Contains("moletom") || nomeNormalizado.Contains("blusa"))
                return "/assets/products/blusa-moletom.jpg";

            // Se o nome tiver Calça ou Jeans, retorna imagem da calça.
            if (nomeNormalizado.Contains("calca") || nomeNormalizado.Contains("calça") || nomeNormalizado.Contains("jeans"))
                return "/assets/products/calca-levis-clara.png";

            // Se o nome tiver Corrente ou Ice, retorna imagem da corrente.
            if (nomeNormalizado.Contains("corrente") || nomeNormalizado.Contains("ice"))
                return "/assets/products/corrente-ice.png";

            // Se o nome tiver Adidas ou Tênis, retorna imagem do tênis.
            if (nomeNormalizado.Contains("adidas") || nomeNormalizado.Contains("tenis") || nomeNormalizado.Contains("tênis"))
                return "/assets/products/tenis2.png";

            // Imagem padrão caso nenhum nome combine com as condições acima.
            return "/assets/products/camisa-over-black.png";
        }

        // Define a marca do produto com base no nome.
        private static string ObterMarca(Produto produto)
        {
            // Normaliza o nome para minúsculo para facilitar a comparação.
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            // Se o nome tiver Nike ou Jordan, considera a marca como Nike.
            if (nomeNormalizado.Contains("nike") || nomeNormalizado.Contains("jordan"))
                return "Nike";

            // Se o nome tiver Adidas, considera a marca como Adidas.
            if (nomeNormalizado.Contains("adidas"))
                return "Adidas";

            // Se o nome tiver Levis ou Levi, considera a marca como Levi's.
            if (nomeNormalizado.Contains("levis") || nomeNormalizado.Contains("levi"))
                return "Levi's";

            // Caso não identifique uma marca conhecida, considera como marca própria da loja.
            return "Zero Frame";
        }

        // Define a origem do produto com base na marca.
        private static string ObterOrigem(Produto produto)
        {
            // Se a marca for Zero Frame, o produto é considerado Original.
            // Caso contrário, é considerado Multimarcas.
            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }
    }
}