using System.Text;
using Azure.Storage.Blobs;
using DoeTech.Configurations;
using DoeTech.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using ApiVersion = Microsoft.AspNetCore.Mvc.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

// Obtêm os dados da aplicação. Obs: se não for encontrado, a aplicação falha ao subir (não opcional).
// Isso não é obirgatório, essa implementação é minha mesmo
builder.Configuration.AddJsonFile("version.json", optional: false, reloadOnChange: true).Build();

// serde JSON C#
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter()); // Serializa todos os enuns para string
    });

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; // Se não especificar a versão, usa a padrão
    options.DefaultApiVersion = new ApiVersion(1, 0); // Versão v1
    options.ReportApiVersions = true; // Mostra no header da resposta as versões disponíveis
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddAuthorization();

// Adiciona a configuração do MySQL a partir do appsettings.json
builder.Services.AddMySqlConfiguration(builder.Configuration);

// Injeção de dependências (para repositórios e services)
builder.Services.AddRepositories();
builder.Services.AddServices();

// Configuração CORS para permitir requisições de origem cruzada do frontend Angular
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    
    options.AddPolicy("OpenPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configurações de envio de email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Configurações do Blob Storage (Azure)
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("BlobStorage");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("BlobStorage connection string is missing or empty.");
    
    Console.WriteLine("BlobStorage connected successfully.");
    return new BlobServiceClient(connectionString);
});


var app = builder.Build();

app.UseCors("OpenPolicy");

// Rodar as migrations de forma automática na base
MySqlConfig.ApplyMigrations(app.Services);

app.MapControllers();

// Configura autenticação
app.UseAuthentication();

// Irá utilizar autenticação em endpoints protegidos
app.UseAuthorization();

// Vamos manipular exceções que lançamos para mapear resposta HTTP válida para o cliente
app.UseMiddleware<ExceptionHandler>();

app.Run();
