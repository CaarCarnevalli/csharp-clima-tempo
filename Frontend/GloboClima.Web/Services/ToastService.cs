// Importações necessárias para o serviço de notificações
using System;

// Define o namespace do serviço
namespace GloboClima.Web.Services;

// Define o serviço de notificações
public class ToastService
{
    // Evento para exibir notificações
    public event Action<string, ToastLevel>? OnShow;

    // Método para exibir notificação de sucesso
    public void ShowSuccess(string message)
    {
        OnShow?.Invoke(message, ToastLevel.Success);
    }

    // Método para exibir notificação de erro
    public void ShowError(string message)
    {
        OnShow?.Invoke(message, ToastLevel.Error);
    }

    // Método para exibir notificação de aviso
    public void ShowWarning(string message)
    {
        OnShow?.Invoke(message, ToastLevel.Warning);
    }

    // Método para exibir notificação de informação
    public void ShowInfo(string message)
    {
        OnShow?.Invoke(message, ToastLevel.Info);
    }
}

// Enumeração para os níveis de notificação
public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}
