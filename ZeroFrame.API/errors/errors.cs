namespace ZeroFrame.API.Errors
{
    public class ApiException
    {
        // recebe um código de status HTTP.
        // Ele retorna as propriedades correspondentes com as informações fornecidas.
        public ApiException(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }

    // Classes específicas para cada tipo de erro,
    public class ApiNotFound : ApiException
    {
        public ApiNotFound(string message, string? details = null)
            : base(StatusCodes.Status404NotFound, message, details)
        {
        }
    }

    // é uma classe específica para representar erros de requisição 
    public class ApiBadRequest : ApiException
    {
        public ApiBadRequest(string message, string? details = null)
            : base(StatusCodes.Status400BadRequest, message, details)
        {
        }
    }
}
