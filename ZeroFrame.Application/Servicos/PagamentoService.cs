using ZeroFrame.Application.DTOS.Pagamento;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do Pagamento.
    // Ele faz a ponte entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPedidoRepository _pedidoRepository;

        // Recebe os repositórios por injeção de dependência.
        public PagamentoService(
            IPagamentoRepository pagamentoRepository,
            IPedidoRepository pedidoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _pedidoRepository = pedidoRepository;
        }

        // Busca um pagamento pelo Id.
        public async Task<PagamentoGetDto?> ObterPorIdAsync(int id)
        {
            var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);

            if (pagamento == null)
                return null;

            return MapearPagamentoGetDto(pagamento);
        }

        // Busca um pagamento pelo Id do pedido.
        public async Task<PagamentoGetDto?> ObterPorPedidoIdAsync(int pedidoId)
        {
            var pagamento = await _pagamentoRepository.ObterPorPedidoIdAsync(pedidoId);

            if (pagamento == null)
                return null;

            return MapearPagamentoGetDto(pagamento);
        }

        // Cria um novo pagamento.
        public async Task<PagamentoGetDto> CriarAsync(PagamentoPostDto pagamentoPostDto)
        {
            return await CriarPagamentoDoPedidoAsync(
                pagamentoPostDto.PedidoId,
                new PagamentoPedidoPostDto { Metodo = pagamentoPostDto.Metodo });
        }

        // Cria um pagamento simples para finalizar um pedido.
        public async Task<PagamentoGetDto> CriarPagamentoDoPedidoAsync(int pedidoId, PagamentoPedidoPostDto pagamentoPedidoPostDto)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                throw new InvalidOperationException("Pedido nao encontrado.");

            if (pedido.Status == "Cancelado")
                throw new InvalidOperationException("Pedido cancelado nao pode receber pagamento.");

            var pagamentoExistente = await _pagamentoRepository.ObterPorPedidoIdAsync(pedidoId);

            if (pagamentoExistente != null)
                throw new InvalidOperationException("Este pedido ja possui pagamento registrado.");

            var pagamento = new Pagamento
            {
                Metodo = pagamentoPedidoPostDto.Metodo,
                PedidoId = pedidoId,
                Pedido = pedido,
                Status = "Aprovado"
            };

            pedido.Status = "Pago";

            await _pagamentoRepository.AdicionarAsync(pagamento);
            await _pedidoRepository.AtualizarAsync(pedido);

            return MapearPagamentoGetDto(pagamento);
        }

        // Atualiza o status do pagamento.
        public async Task AtualizarAsync(PagamentoPutDto pagamentoPutDto)
        {
            var pagamento = await _pagamentoRepository.ObterPorIdAsync(pagamentoPutDto.Id);

            if (pagamento == null)
                return;

            pagamento.Status = pagamentoPutDto.Status;

            await _pagamentoRepository.AtualizarAsync(pagamento);
        }

        private static PagamentoGetDto MapearPagamentoGetDto(Pagamento pagamento)
        {
            return new PagamentoGetDto
            {
                Id = pagamento.Id,
                Metodo = pagamento.Metodo,
                Status = pagamento.Status,
                PedidoId = pagamento.PedidoId,
                Valor = pagamento.Pedido?.ValorTotal ?? 0m
            };
        }
    }
}