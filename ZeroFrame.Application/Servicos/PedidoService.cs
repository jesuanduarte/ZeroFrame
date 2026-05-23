using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Enums;
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
        private readonly IFreteService _freteService;
        private static readonly string[] StatusEntregaPermitidos = ["Pendente", "Em Separacao", "Enviado", "Entregue", "Cancelado"];

        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IVariacaoRepository variacaoRepository,
            IUsuarioRepository usuarioRepository,
            IEnderecoRepository enderecoRepository,
            IUnitOfWork unitOfWork,
            IFreteService freteService)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _variacaoRepository = variacaoRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
            _unitOfWork = unitOfWork;
            _freteService = freteService;
        }

        
        // Busca um pedido pelo Id.

        public async Task<List<PedidosGetDto>> ObterTodosAsync()
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            return pedidos.Select(MapearPedidoGetDto).ToList();
        }

        public async Task<PagedResponse<PedidosGetDto>> ObterTodosPaginadoAsync(PaginationParams paginationParams)
        {
            var resultado = await _pedidoRepository.ObterTodosPaginadoAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var items = resultado.Items.Select(MapearPedidoGetDto).ToList();
            return PagedResponse<PedidosGetDto>.Create(items, resultado.TotalItems, paginationParams);
        }

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

        public async Task<PagedResponse<PedidosGetDto>> ObterPorUsuarioPaginadoAsync(
            int usuarioId,
            PaginationParams paginationParams)
        {
            var resultado = await _pedidoRepository.ObterPorUsuarioPaginadoAsync(
                usuarioId,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var items = resultado.Items.Select(MapearPedidoGetDto).ToList();
            return PagedResponse<PedidosGetDto>.Create(items, resultado.TotalItems, paginationParams);
        }

        // Cria um pedido diretamente a partir do DTO recebido.
        public async Task<PedidosGetDto> CriarAsync(PedidosPostDto pedidosPostDto)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var endereco = await ValidarUsuarioEEnderecoAsync(
                    pedidosPostDto.UsuarioId,
                    pedidosPostDto.EnderecoId);

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

                    var precoVenda = ProdutoPrecoService.CalcularPrecoFinal(variacao.Produto);

                    itensPedido.Add(new ItemPedido
                    {
                        VariacaoProdutoId = itemDto.VariacaoProdutoId,
                        VariacaoProduto = variacao,
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitario = precoVenda,
                        PrecoCustoUnitario = variacao.Produto.PrecoCusto
                    });
                }

                var pedido = new Pedidos
                {
                    UsuarioId = pedidosPostDto.UsuarioId,
                    EnderecoId = pedidosPostDto.EnderecoId,
                    Endereco = endereco,
                    DataPedido = DateTime.UtcNow,
                    Status = StatusPedido.Pendente,
                    StatusEntrega = "Pendente",
                    Itens = itensPedido
                };

                var subtotalPedido = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
                pedido.ValorTotal = subtotalPedido + _freteService.CalcularFrete(subtotalPedido);

                await _pedidoRepository.CriarPedidoAsync(pedido);
                return MapearPedidoGetDto(pedido);
            });
        }

        // Cria o pedido com os itens montados.
        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAsync(int carrinhoId, int enderecoId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var carrinho = await _carrinhoRepository.ObterPorIdAsync(carrinhoId);

                if (carrinho == null)
                    throw new InvalidOperationException("Carrinho não encontrado.");

                var endereco = await ValidarUsuarioEEnderecoAsync(carrinho.UsuarioId, enderecoId);

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

                    var precoVenda = ProdutoPrecoService.CalcularPrecoFinal(variacao.Produto);

                    itensPedido.Add(new ItemPedido
                    {
                        VariacaoProdutoId = itemCarrinho.VariacaoProdutoId,
                        VariacaoProduto = variacao,
                        Quantidade = itemCarrinho.Quantidade,
                        PrecoUnitario = precoVenda,
                        PrecoCustoUnitario = variacao.Produto.PrecoCusto
                    });
                }

                // Cria o pedido com os itens montados.
                var pedido = new Pedidos
                {
                    UsuarioId = carrinho.UsuarioId,
                    EnderecoId = enderecoId,
                    Endereco = endereco,
                    DataPedido = DateTime.UtcNow,
                    Status = StatusPedido.Pendente,
                    StatusEntrega = "Pendente",
                    Itens = itensPedido
                };

                var subtotalPedido = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
                pedido.ValorTotal = subtotalPedido + _freteService.CalcularFrete(subtotalPedido);
                carrinho.Ativo = false;

                await _pedidoRepository.CriarPedidoAsync(pedido);
                await _carrinhoRepository.AtualizarAsync(carrinho);

                return MapearPedidoGetDto(pedido);
            });
        }

        // Busca todos os pedidos de um usuário específico.
        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(int usuarioId, int enderecoId)
        {
            var carrinho = await _carrinhoRepository.ObterAtivoPorUsuarioAsync(usuarioId);

            if (carrinho == null)
                throw new InvalidOperationException("Carrinho ativo não encontrado para este usuário.");

            return await CriarAPartirDoCarrinhoAsync(carrinho.Id, enderecoId);
        }

        // metodo para cancelar um pedido, atualizando o estoque das variações dos produtos e o status do pedido.
        public async Task CancelarAsync(int id)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);

                if (pedido == null)
                    return;

                if (pedido.Status == StatusPedido.Cancelado)
                    return;

                ValidarAlteracaoStatus(pedido.Status, StatusPedido.Cancelado);

                foreach (var item in pedido.Itens)
                {
                    var variacao = await _variacaoRepository.ObterPorIdAsync(item.VariacaoProdutoId);

                    if (variacao != null)
                    {
                        variacao.Estoque += item.Quantidade;
                        await _variacaoRepository.AtualizarAsync(variacao);
                    }
                }

                pedido.Status = StatusPedido.Cancelado;
                pedido.StatusEntrega = "Cancelado";
                await _pedidoRepository.AtualizarAsync(pedido);
            });
        }

        public async Task AtualizarStatusAsync(int pedidoId, PedidoStatusUpdateDto pedidoStatusUpdateDto, bool usuarioAdministrador)
        {
            if (!usuarioAdministrador)
                throw new UnauthorizedAccessException("Apenas Administrador pode alterar o status do pedido.");

            if (!Enum.IsDefined(typeof(StatusPedido), pedidoStatusUpdateDto.Status))
                throw new InvalidOperationException("Status do pedido invalido.");

            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                throw new KeyNotFoundException("Pedido nao encontrado.");

            ValidarAlteracaoStatus(pedido.Status, pedidoStatusUpdateDto.Status);

            pedido.Status = pedidoStatusUpdateDto.Status;
            await _pedidoRepository.AtualizarAsync(pedido);
        }

        public async Task AtualizarStatusEntregaAsync(int pedidoId, PedidoStatusEntregaUpdateDto pedidoStatusEntregaUpdateDto, bool usuarioAdministrador)
        {
            if (!usuarioAdministrador)
                throw new UnauthorizedAccessException("Apenas Administrador pode alterar o status de entrega do pedido.");

            var statusEntrega = NormalizarStatusEntrega(pedidoStatusEntregaUpdateDto.StatusEntrega);
            if (!StatusEntregaPermitidos.Contains(statusEntrega))
                throw new InvalidOperationException("Status de entrega invalido.");

            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedido == null)
                throw new KeyNotFoundException("Pedido nao encontrado.");

            pedido.StatusEntrega = statusEntrega;
            pedido.PrevisaoEntrega = pedidoStatusEntregaUpdateDto.PrevisaoEntrega;

            // DataEntrega representa o momento real da conclusao e fica em UTC para relatorios consistentes.
            pedido.DataEntrega = statusEntrega == "Entregue"
                ? DateTime.UtcNow
                : pedido.DataEntrega;

            if (statusEntrega == "Cancelado")
                pedido.Status = StatusPedido.Cancelado;

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
        private async Task<Endereco> ValidarUsuarioEEnderecoAsync(int usuarioId, int enderecoId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                throw new InvalidOperationException("Usuario nao encontrado.");

            var endereco = await _enderecoRepository.ObterPorIdAsync(enderecoId);

            if (endereco == null || !endereco.Ativo)
                throw new InvalidOperationException("Endereço não encontrado.");

            if (endereco.UsuarioId != usuarioId)
                throw new InvalidOperationException("Este endereço não pertence ao usuário informado.");

            return endereco;
        }

        // Mapeia a entidade Pedidos para o DTO PedidosGetDto,
        // incluindo o cálculo do subtotal, desconto, frete e valor total.
        private PedidosGetDto MapearPedidoGetDto(Pedidos pedido)
        {
            var itens = pedido.Itens.Select(MapearItemPedidoGetDto).ToList();
            var subtotal = itens.Sum(item => item.Subtotal);
            var desconto = 0m;
            var frete = _freteService.CalcularFrete(subtotal - desconto);
            var valorTotalComFrete = subtotal - desconto + frete;

            return new PedidosGetDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                Usuario = MapearUsuarioGetDto(pedido.Usuario, pedido.UsuarioId),
                EnderecoId = pedido.EnderecoId,
                Endereco = MapearEnderecoGetDto(pedido.Endereco, pedido.EnderecoId),
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                MensagemStatus = ObterMensagemStatus(pedido.Status),
                StatusEntrega = pedido.StatusEntrega,
                PrevisaoEntrega = pedido.PrevisaoEntrega,
                DataEntrega = pedido.DataEntrega,
                TotalItens = itens.Sum(item => item.Quantidade),
                Subtotal = subtotal,
                Desconto = desconto,
                Frete = frete,
                ValorFrete = frete,
                ValorTotal = valorTotalComFrete,
                ValorTotalComFrete = valorTotalComFrete,
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
                PrecoCustoUnitario = item.PrecoCustoUnitario,
                Subtotal = item.Quantidade * item.PrecoUnitario,
                Produto = produto == null
                    ? new ItemPedidoProdutoGetDto()
                    : new ItemPedidoProdutoGetDto
                    {
                        ProdutoId = produto.Id,
                        Nome = produto.Nome,
                        Preco = produto.Preco,
                        PrecoCusto = produto.PrecoCusto,
                        ImagemUrl = ObterImagemUrl(produto),
                        Categoria = produto.Categoria?.Nome ?? string.Empty,
                        Marca = ObterMarca(produto),
                        Origem = ObterOrigem(produto)
                    }
            };
        }

        private static PedidoUsuarioGetDto MapearUsuarioGetDto(Usuario? usuario, int usuarioId)
        {
            if (usuario == null)
                return new PedidoUsuarioGetDto { Id = usuarioId };

            return new PedidoUsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        // Mapeia o endereco de entrega salvo no pedido para o retorno da API.
        private static PedidoEnderecoGetDto MapearEnderecoGetDto(Endereco? endereco, int enderecoId)
        {
            if (endereco == null)
                return new PedidoEnderecoGetDto { EnderecoId = enderecoId };

            return new PedidoEnderecoGetDto
            {
                EnderecoId = endereco.Id,
                Rua = endereco.Rua,
                Numero = endereco.Numero,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.CEP,
                Complemento = endereco.Complemento
            };
        }

        private static void ValidarAlteracaoStatus(StatusPedido statusAtual, StatusPedido novoStatus)
        {
            if (statusAtual == novoStatus)
                return;

            if (statusAtual is StatusPedido.Entregue or StatusPedido.Cancelado)
                throw new InvalidOperationException("Nao e permitido alterar um pedido entregue ou cancelado.");

            var alteracaoPermitida = statusAtual switch
            {
                StatusPedido.Pendente => novoStatus is StatusPedido.PreparandoParaSaida or StatusPedido.Cancelado,
                StatusPedido.PreparandoParaSaida => novoStatus is StatusPedido.SaiuParaEntrega or StatusPedido.Cancelado,
                StatusPedido.SaiuParaEntrega => novoStatus is StatusPedido.Entregue or StatusPedido.Cancelado,
                _ => false
            };

            if (!alteracaoPermitida)
                throw new InvalidOperationException("Alteracao de status do pedido nao permitida.");
        }

        private static string NormalizarStatusEntrega(string statusEntrega)
        {
            var status = statusEntrega.Trim();

            return StatusEntregaPermitidos
                .FirstOrDefault(statusPermitido => statusPermitido.Equals(status, StringComparison.OrdinalIgnoreCase))
                ?? status;
        }

        private static string ObterMensagemStatus(StatusPedido status)
        {
            return status switch
            {
                StatusPedido.Pendente => "Seu pedido está pendente.",
                StatusPedido.PreparandoParaSaida => "Seu pedido está sendo preparado para saída.",
                StatusPedido.SaiuParaEntrega => "Seu pedido saiu para rota de entrega.",
                StatusPedido.Entregue => "Seu pedido foi entregue com sucesso.",
                StatusPedido.Cancelado => "Seu pedido foi cancelado.",
                _ => "Status do pedido invalido."
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
