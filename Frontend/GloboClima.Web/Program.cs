// Importações necessárias para o programa
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using GloboClima.Web.Services; // Nosso namespace de serviços
using GloboClima.Web.Auth; // Namespace para CustomAuthenticationStateProvider
using System.Globalization;
using Blazored.Toast;

// Define o namespace do programa
namespace GloboClima.Web;

// Define o programa
public class Program
{
    // Método principal
    public static async Task Main(string[] args)
    {
        // Cria o construtor do host
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Adiciona os componentes raiz
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Configuração da cultura para pt-BR
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

        // Configurar HttpClient para a API do Backend
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5146/") });
        builder.Services.AddHttpClient();

        // Adicionar Blazored.LocalStorage
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddBlazoredToast();

        // Adicionar serviços de autenticação
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        // Adicionar serviços da aplicação
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<WeatherService>();
        builder.Services.AddScoped<CountryService>();
        builder.Services.AddScoped<FavoriteService>();
        builder.Services.AddSingleton<ToastService>();

        builder.Services.AddOptions();

        // Executa o programa
        await builder.Build().RunAsync();
    }
}
