# GloboClima - Aplicação de Clima e Países

## Visão Geral
GloboClima é uma aplicação web moderna que permite aos usuários consultar informações meteorológicas atualizadas de cidades ao redor do mundo e obter dados detalhados sobre países. A aplicação implementa autenticação de usuários via JWT e permite que usuários autenticados salvem e gerenciem uma lista de cidades favoritas, com persistência de dados utilizando AWS DynamoDB (atualmente com um mock local para desenvolvimento).

## Estrutura do Projeto
O projeto é dividido em duas partes principais: 
- um backend construído com ASP.NET Core API
- E um frontend com Blazor WebAssembly.

```
GloboClima/
├── GloboClima.sln               # Arquivo de Solução .NET
├── Backend/
│   └── GloboClima.API/          # Projeto da API RESTful
│       ├── Controllers/         # Controladores para as rotas da API (Auth, Country, Favorites, Weather)
│       ├── Domain/              # Entidades de domínio (ex: Favorite)
│       ├── DTOs/                # Data Transfer Objects para as requisições e respostas da API
│       ├── Interfaces/          # Contratos para serviços e repositórios
│       ├── Models/              # Modelos de dados internos (ex: User)
│       ├── Repositories/        # Camada de acesso a dados (DynamoDbFavoriteRepository, PlaceholderFavoriteRepository)
│       ├── Services/            # Lógica de negócio, incluindo consumo de APIs externas (ExternalApiServices, FavoriteService)
│       ├── Auth/                # (Atualmente vazia, lógica de JWT no AuthController e Program.cs)
│       ├── appsettings.json     # Configurações da aplicação, incluindo chaves de API e config. DynamoDB
│       └── Program.cs           # Configuração da aplicação: serviços, DI, CORS, JWT, Swagger
└── Frontend/
    └── GloboClima.Web/          # Projeto Blazor WebAssembly (Client-Side)
        ├── Auth/                # Lógica de autenticação client-side (CustomAuthenticationStateProvider)
        ├── Models/              # DTOs e modelos de visualização para o frontend
        ├── Pages/               # Componentes Razor para as páginas da aplicação (Home, Login, Register, WeatherSearch, CountrySearch, Favorites, etc.)
        ├── Services/            # Serviços para interagir com a API backend (AuthService, CountryService, FavoriteService, WeatherService, ToastService)
        ├── Shared/              # Componentes Razor compartilhados (MainLayout, NavMenu, Toast)
        ├── wwwroot/             # Arquivos estáticos (index.html, CSS, imagens, sample-data)
        │   ├── css/app.css      # Estilos globais da aplicação
        │   └── index.html       # Página HTML principal do Blazor Wasm
        ├── _Imports.razor       # Diretivas @using globais para o projeto Blazor
        ├── App.razor            # Componente raiz da aplicação Blazor
        └── Program.cs           # Configuração do cliente Blazor: HttpClient, serviços, Blazored.LocalStorage
```

## Funcionalidades Principais
- **Consulta de Clima:** Busca e exibe informações meteorológicas detalhadas por cidade (temperatura, sensação térmica, umidade, vento, etc.), consumindo a API OpenWeatherMap.
- **Informações de Países:** Busca e exibe dados sobre países (capital, população, região, moeda, bandeira, etc.), consumindo a API RestCountries.
- **Autenticação de Usuários:**
    - Registro de novos usuários.
    - Login com credenciais de usuário.
    - Implementação de JSON Web Tokens (JWT) para autenticação segura das requisições à API.
- **Gerenciamento de Favoritos (para usuários autenticados):**
    - Adicionar cidades à lista de favoritos.
    - Visualizar a lista de cidades favoritas.
    - Remover cidades da lista de favoritos.
    - Persistência dos favoritos utilizando AWS DynamoDB (com fallback para um repositório em memória para desenvolvimento).
- **Interface Responsiva e Amigável:** Interface construída com Blazor WebAssembly e Bootstrap, proporcionando uma boa experiência em diferentes dispositivos.
- **Notificações (Toasts):** Exibição de mensagens de feedback para o usuário (sucesso, erro, informação).

## Tecnologias Utilizadas
- **Backend:**
    - ASP.NET Core 8.0 (API RESTful)
    - JWT (JSON Web Token) para Autenticação
    - AWS SDK para .NET (DynamoDB)
    - Swagger/OpenAPI para documentação da API
    - Injeção de Dependência (DI)
- **Frontend:**
    - Blazor WebAssembly (.NET 8.0)
    - Bootstrap 5 para estilização e responsividade
    - Blazored.LocalStorage para armazenamento de tokens JWT no navegador
    - Serviços HttpClient para consumo da API backend
- **Bando de Dados:**
    - AWS DynamoDB (para persistência de favoritos)
    - Repositório em memória (`PlaceholderFavoriteRepository`) como alternativa para desenvolvimento local sem DynamoDB configurado.
- **APIs Externas Consumidas:**
    - OpenWeatherMap API (para dados de clima)
    - RestCountries API (para dados de países)
- **Linguagem:**
    - C# em todo o stack

## Como Executar o Projeto

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) Configuração da AWS CLI com credenciais válidas e acesso ao DynamoDB, se desejar usar a persistência real. Caso contrário, o projeto utiliza um repositório em memória.
- Chaves de API para OpenWeatherMap e, se necessário, outras APIs externas (configurar em `Backend/GloboClima.API/appsettings.json`).

### Backend (GloboClima.API)
1.  Navegue até a pasta do backend:
    ```bash
    cd Backend/GloboClima.API
    ```
2.  (Opcional) Configure suas chaves de API e configurações da AWS no arquivo `appsettings.json` ou via user secrets.
    Exemplo de `appsettings.json` para chaves de API:
    ```json
    {
      "Logging": { /* ... */ },
      "AllowedHosts": "*",
      "ApiKeys": {
        "OpenWeatherMap": "SUA_CHAVE_OPENWEATHERMAP_AQUI"
      },
      "AWS": {
        "Region": "sua-regiao-aws", // ex: "us-east-1"
        "DynamoDB": {
          "FavoritesTable": "GloboClimaFavorites"
        }
      }
      // ... outras configurações
    }
    ```
3.  Execute a API:
    ```bash
    dotnet run
    ```
    Por padrão, o backend estará disponível em `http://localhost:5146`.
    A documentação Swagger da API pode ser acessada em `http://localhost:5146/swagger`.

### Frontend (GloboClima.Web)
1.  Navegue até a pasta do frontend:
    ```bash
    cd Frontend/GloboClima.Web
    ```
2.  Execute a aplicação Blazor:
    ```bash
    dotnet run
    ```
    Por padrão, o frontend estará disponível em `http://localhost:5206`.

## Usuários de Teste (Backend API)
A API possui uma lista estática de usuários para fins de demonstração (configurada em `AuthController.cs`):
- Usuário 1: `usuario1` / Senha: `Senha123`
- Usuário 2: `usuario2` / Senha: `Senha123`
- Usuário 3: `usuario3` / Senha: `Senha123`

Estes usuários podem ser usados para testar o login e as funcionalidades de favoritos.

## Próximos Passos e Melhorias Potenciais
- Implementar um sistema de gerenciamento de usuários mais robusto (ex: ASP.NET Core Identity).
- Adicionar mais testes unitários e de integração.
- Melhorar o tratamento de erros e resiliência nas chamadas a APIs externas (ex: Polly).
- Implementar cacheamento para dados de APIs externas no backend.
- Refinar a interface do usuário e experiência do usuário (UX).
- Configurar um pipeline de CI/CD.
