using ZeroFrame.Application.DTOS.Pagamento;
using ZeroFrame.Application.DTOS.Usuario;
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

        // Recebe o repositório por injeção de dependência.
        public PagamentoService(IPagamentoRepository pagamentoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
        }

        // Busca um pagamento pelo Id.
        public async Task<PagamentoGetDto?> ObterPorIdAsync(int id)
        {
            var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);

            if (pagamento == null)
                return null;

            return new PagamentoGetDto
            {
                Id = pagamento.Id,
                Metodo = pagamento.Metodo,
                Status = pagamento.Status,
                PedidoId = pagamento.PedidoId
            };
        }

        // Busca um pagamento pelo Id do pedido.
        public async Task<PagamentoGetDto?> ObterPorPedidoIdAsync(int pedidoId)
        {
            var pagamento = await _pagamentoRepository.ObterPorPedidoIdAsync(pedidoId);

            if (pagamento == null)
                return null;

            return new PagamentoGetDto
            {
                Id = pagamento.Id,
                Metodo = pagamento.Metodo,
                Status = pagamento.Status,
                PedidoId = pagamento.PedidoId
            };
        }

        // Cria um novo pagamento.
        public async Task<PagamentoGetDto> CriarAsync(PagamentoPostDto pagamentoPostDto)
        {
            var pagamento = new Pagamento
            {
                Metodo = pagamentoPostDto.Metodo,
                PedidoId = pagamentoPostDto.PedidoId,
                Status = "Pendente"
            };

            await _pagamentoRepository.AdicionarAsync(pagamento);

            return new PagamentoGetDto
            {
                Id = pagamento.Id,
                Metodo = pagamento.Metodo,
                Status = pagamento.Status,
                PedidoId = pagamento.PedidoId
            };
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
    }
}