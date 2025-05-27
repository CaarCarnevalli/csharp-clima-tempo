// Define o namespace para os modelos da aplicação web.
namespace GloboClima.Web.Models;

// Representa uma resposta padrão da API, indicando sucesso ou falha e uma mensagem de erro opcional.
public class ApiResponse
{
    // Indica se a operação da API foi bem-sucedida.
    public bool IsSuccess { get; set; }
    // Mensagem de erro, caso a operação tenha falhado.
    public string? ErrorMessage { get; set; }
    // Código de status HTTP da resposta da API.
    public int StatusCode { get; set; }
} 