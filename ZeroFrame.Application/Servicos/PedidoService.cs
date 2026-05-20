using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IVariacaoRepository _variacaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IVariacaoRepository variacaoRepository,
            IUsuarioRepository usuarioRepository,
            IEnderecoRepository enderecoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _variacaoRepository = variacaoRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
            _unitOfWork = unitOfWork;
        }

        
        // Busca um pedido pelo Id.

        public async Task<PedidosGetDto?> ObterPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return null;

            return MapearPedidoGetDto(pedido);
        }

        // Busca todos os pedidos de um usuário específico.
        public async Task<List<PedidosGetDto>> ObterPorUsuarioAsync(int usuarioId)
        {
            var pedidos = await _pedidoRepository.ObterPorUsuarioAsync(usuarioId);
            return pedidos.Select(MapearPedidoGetDto).ToList();
        }

        // Cria um pedido diretamente a partir do DTO recebido.
        public async Task<PedidosGetDto> CriarAsync(PedidosPostDto pedidosPostDto)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await ValidarUsuarioEEnderecoAsync(pedidosPostDto.UsuarioId);

                var itensPedido = new List<ItemPedido>();
            
                // Percorre os itens enviados no pedido.

                foreach (var itemDto in pedidosPostDto.Itens)
                {
                    var variacao = await _variacaoRepository.ObterPorIdAsync(itemDto.VariacaoProdutoId);

                    if (variacao == null)
                        throw new InvalidOperationException("Variação do produto não encontrada.");

                    if (variacao.Produto == null)
                        throw new InvalidOperationException("Produto da variação não encontrado.");

                    ValidarEstoque(variacao, itemDto.Quantidade);

                    variacao.Estoque -= itemDto.Quantidade;
                    await _variacaoRepository.AtualizarAsync(variacao);

                    itensPedido.Add(new ItemPedido
                    {
                        VariacaoProdutoId = itemDto.VariacaoProdutoId,
                        VariacaoProduto = variacao,
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitario = variacao.Produto.Preco
                    });
                }

                var pedido = new Pedidos
                {
                    UsuarioId = pedidosPostDto.UsuarioId,
                    DataPedido = DateTime.Now,
                    Status = "Pendente",
                    Itens = itensPedido
                };

                pedido.ValorTotal = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);

                await _pedidoRepository.CriarPedidoAsync(pedido);
                return MapearPedidoGetDto(pedido);
            });
        }

        // Cria o pedido com os itens montados.
        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAsync(int carrinhoId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var carrinho = await _carrinhoRepository.ObterPorIdAsync(carrinhoId);

                if (carrinho == null)
                    throw new InvalidOperationException("Carrinho não encontrado.");

                await ValidarUsuarioEEnderecoAsync(carrinho.UsuarioId);

                if (!carrinho.Ativo)
                    throw new InvalidOperationException("Carrinho inativo não pode gerar pedido.");

                if (!carrinho.Itens.Any())
                    throw new InvalidOperationException("Carrinho vazio não pode gerar pedido.");

                var itensPedido = new List<ItemPedido>();

                foreach (var itemCarrinho in carrinho.Itens)
                {
                    var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinho.VariacaoProdutoId);

                    if (variacao == null)
                        throw new InvalidOperationException("Variação do produto não encontrada.");

                    if (variacao.Produto == null)
                        throw new InvalidOperationException("Produto da variação não encontrado.");

                    ValidarEstoque(variacao, itemCarrinho.Quantidade);

                    variacao.Estoque -= itemCarrinho.Quantidade;
                    await _variacaoRepository.AtualizarAsync(variacao);

                    itensPedido.Add(new ItemPedido
                    {
                        VariacaoProdutoId = itemCarrinho.VariacaoProdutoId,
                        VariacaoProduto = variacao,
                        Quantidade = itemCarrinho.Quantidade,
                        PrecoUnitario = variacao.Produto.Preco
                    });
                }

                // Cria o pedido com os itens montados.
                var pedido = new Pedidos
                {
                    UsuarioId = carrinho.UsuarioId,
                    DataPedido = DateTime.Now,
                    Status = "Pendente",
                    Itens = itensPedido
                };

                pedido.ValorTotal = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
                carrinho.Ativo = false;

                await _pedidoRepository.CriarPedidoAsync(pedido);
                await _carrinhoRepository.AtualizarAsync(carrinho);

                return MapearPedidoGetDto(pedido);
            });
        }

        // Busca todos os pedidos de um usuário específico.
        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(int usuarioId)
        {
            var carrinho = await _carrinhoRepository.ObterAtivoPorUsuarioAsync(usuarioId);

            if (carrinho == null)
                throw new InvalidOperationException("Carrinho ativo não encontrado para este usuário.");

            return await CriarAPartirDoCarrinhoAsync(carrinho.Id);
        }

        // metodo para cancelar um pedido, atualizando o estoque das variações dos produtos e o status do pedido.
        public async Task CancelarAsync(int id)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);

                if (pedido == null)
                    return;

                if (pedido.Status == "Cancelado")
                    return;

                foreach (var item in pedido.Itens)
                {
                    var variacao = await _variacaoRepository.ObterPorIdAsync(item.VariacaoProdutoId);

                    if (variacao != null)
                    {
                        variacao.Estoque += item.Quantidade;
                        await _variacaoRepository.AtualizarAsync(variacao);
                    }
                }

                pedido.Status = "Cancelado";
                await _pedidoRepository.AtualizarAsync(pedido);
            });
        }

        // Atualiza o status do pedido.
        public async Task AtualizarAsync(PedidosPutDto pedidosPutDto)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidosPutDto.Id);

            if (pedido == null)
                return;

            pedido.Status = pedidosPutDto.Status;
            await _pedidoRepository.AtualizarAsync(pedido);
        }

        // Valida o estoque antes de criar ou atualizar um pedido.
        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variação.");
        }

        // Valida os dados mínimos do checkout antes de criar o pedido e baixar estoque.
        private async Task ValidarUsuarioEEnderecoAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                throw new InvalidOperationException("Usuario nao encontrado.");

            var endereco = await _enderecoRepository.ObterPorUsuarioIdAsync(usuarioId);

            if (endereco == null || !endereco.Ativo)
                throw new InvalidOperationException("Endereco valido nao encontrado para este usuario.");
        }

        // Mapeia a entidade Pedidos para o DTO PedidosGetDto,
        // incluindo o cálculo do subtotal, desconto, frete e valor total.
        private static PedidosGetDto MapearPedidoGetDto(Pedidos pedido)
        {
            var itens = pedido.Itens.Select(MapearItemPedidoGetDto).ToList();
            var subtotal = itens.Sum(item => item.Subtotal);
            var desconto = 0m;
            var frete = 0m;
            var valorTotal = pedido.ValorTotal > 0 ? pedido.ValorTotal : subtotal - desconto + frete;

            return new PedidosGetDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                TotalItens = itens.Sum(item => item.Quantidade),
                Subtotal = subtotal,
                Desconto = desconto,
                Frete = frete,
                ValorTotal = valorTotal,
                Itens = itens
            };
        }

        // Mapeia a entidade ItemPedido para o DTO ItemPedidoGetDto,
        // incluindo a obtenção da URL da imagem, marca e origem do produto.
        private static ItemPedidoGetDto MapearItemPedidoGetDto(ItemPedido item)
        {
            var variacao = item.VariacaoProduto;
            var produto = variacao?.Produto;

            return new ItemPedidoGetDto
            {
                Id = item.Id,
                PedidoId = item.PedidoId,
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

        // Obtém a URL da imagem com base no nome do produto,
        // utilizando palavras-chave para identificar o tipo de produto.
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

        // Obtém a marca do produto com base no nome,
        // utilizando palavras-chave para identificar a marca.
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

        // Obtém a origem do produto com base na marca,
        // utilizando a marca para determinar se é original ou multimarcas.
        private static string ObterOrigem(Produto produto)
        {
            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }
    }
}
