using System.Text;
using Validacao.Aplication.Service;
using Validacao.Application.Interfaces;
using Validacao.Application.Interfaces.Repositories;
using Validacao.Infrastructure.Repositories;
using Validacao.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7292")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? string.Empty;
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    // CORRIGIDO: Usando as constantes corretas do JwtBearerDefaults
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Essencial para os Controllers funcionarem!

// CORRIGIDO: Registro tipificado do HttpClient para o FreteService
builder.Services.AddHttpClient<FreteService>(client =>
{
    // Configure aqui a URL base da API externa de CEP, caso o FreteService faça chamadas externas
    // client.BaseAddress = new Uri("https://viacep.com.br/"); 
});

// Registros de Serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ICriptografiaServico, CriptografiaServico>();
builder.Services.AddScoped<IValidadorService, ValidadorService>();
builder.Services.AddScoped<IEmailService, EmailServico>();
builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFreteService, FreteService>();

string urlSupabase = builder.Configuration["Supabase:Url"] ?? string.Empty;
string chaveSupabase = builder.Configuration["Supabase:Chave"] ?? string.Empty;

if (string.IsNullOrEmpty(urlSupabase) || string.IsNullOrEmpty(chaveSupabase))
{
    throw new InvalidOperationException("ERRO CRÍTICO: As credenciais do Supabase não foram encontradas no appsettings.json da API!");
}

var opcoes = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Supabase.Client(urlSupabase, chaveSupabase, opcoes));

var app = builder.Build();

// Pipeline de execução do HTTP (Middlewares)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: O UseCors DEVE vir antes da Autenticação/Autorização e rotas
app.UseCors("LiberarBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/", () => "Minha API de CEP está online e organizada! 🚀");

// O seu MapGet de CEP continua funcionando perfeitamente aqui
app.MapGet("/cep/{numeroCep}", async (string numeroCep, FreteService freteService) =>
{
    var resultado = await freteService.CalcularFreteAsync(numeroCep);

    if (resultado == null)
    {
        return Results.NotFound(new { mensagem = "CEP não encontrado ou inválido." });
    }

    return Results.Ok(resultado);
});

app.MapControllers();

app.Run();