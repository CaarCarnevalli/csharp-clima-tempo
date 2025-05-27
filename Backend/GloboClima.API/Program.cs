// Importações necessárias para o funcionamento do backend
using Amazon.DynamoDBv2;
using GloboClima.API.Interfaces;
using GloboClima.API.Services;
using GloboClima.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Collections.Generic;

// Configuração inicial da aplicação
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Adiciona os controladores ao contêiner de serviços
builder.Services.AddControllers();

// Configura o CORS para permitir todas as origens, métodos e cabeçalhos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configura o HttpClient para chamadas a APIs externas
builder.Services.AddHttpClient();

// Registra os serviços da aplicação
builder.Services.AddScoped<IWeatherService, ExternalApiServices>();
builder.Services.AddScoped<ICountryService, ExternalApiServices>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// Configura os serviços da AWS
builder.Services.AddDefaultAWSOptions(configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();

// Registra os repositórios
// Use DynamoDbFavoriteRepository (produção) ou PlaceholderFavoriteRepository (local/dev sem AWS)
builder.Services.AddSingleton<IFavoriteRepository, PlaceholderFavoriteRepository>(); // <- use esse se ainda não tem o DynamoDB
// builder.Services.AddSingleton<IFavoriteRepository, DynamoDbFavoriteRepository>(); // <- ativado para integração com DynamoDB

// Configura a autenticação JWT
var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured in appsettings.Development.json or appsettings.json.");
var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured.");
var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured.");

// Adiciona a autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Adiciona a autorização
builder.Services.AddAuthorization();

// Configura o Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GloboClima API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Constrói a aplicação
var app = builder.Build();

// Configura o pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GloboClima API V1");
    });
    app.UseDeveloperExceptionPage();
}

// Comentado para permitir comunicação HTTP com o frontend
// app.UseHttpsRedirection();

// Usa a política CORS configurada
app.UseCors("AllowAll");

// Usa autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os controladores
app.MapControllers();

// Executa a aplicação
app.Run();
