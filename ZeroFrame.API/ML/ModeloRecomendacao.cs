using Microsoft.ML.Data;

namespace ZeroFrame.API.ML
{
    public class DadosPedido
    {
        [KeyType(count: 1000)]
        public uint UsuarioId { get; set; }

        [KeyType(count: 1000)]
        public uint ProdutoId { get; set; }

        public float Avaliacao { get; set; }
    }

    public class PrevisaoRecomendacao
    {
        public float Score { get; set; }
    }
}
