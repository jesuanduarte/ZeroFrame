using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZeroFrame.API.Errors;

namespace ZeroFrame.API.Filters
{
    public class ApiErrorResultFilter : IActionFilter
    {
        // O filtro verifica se o resultado da ação é um BadRequest ou NotFound
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        // Se for um BadRequest, ele converte o valor para um ApiBadRequest usando o método ToApiBadRequest.
        // Se for um NotFound, ele converte o valor para um ApiNotFound usando o método ToApiNotFound.
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is BadRequestObjectResult badRequest)
            {
                badRequest.Value = ToApiBadRequest(badRequest.Value);
                return;
            }

            if (context.Result is NotFoundObjectResult notFound)
            {
                notFound.Value = ToApiNotFound(notFound.Value);
            }
        }

        // Os métodos do ApiBadRequest e ApiNotFound verificam do tipo ApiBadRequest ou ApiNotFound, respectivamente.
        // Se não for, eles criam uma nova instância correspondente com a mensagem apropriada.
        private static ApiBadRequest ToApiBadRequest(object? value)
        {
            return value is ApiBadRequest apiBadRequest
                ? apiBadRequest
                : new ApiBadRequest(GetMessage(value, "ação invalida."));
        }


        //  cria um ApiNotFound com uma mensagem padrão.
        private static ApiNotFound ToApiNotFound(object? value)
        {
            return value is ApiNotFound apiNotFound
                ? apiNotFound
                : new ApiNotFound(GetMessage(value, "Recurso nao encontrado."));
        }

        //Se não encontrar uma mensagem válida, ele retorna a mensagem fornecida valida.
        private static string GetMessage(object? value, string fallbackMessage)
        {
            if (value is string message)
            {
                return message;
            }

            var mensagemProperty = value?.GetType().GetProperty("mensagem");
            var mensagemValue = mensagemProperty?.GetValue(value)?.ToString();

            return string.IsNullOrWhiteSpace(mensagemValue)
                ? fallbackMessage
                : mensagemValue;
        }
    }
}
