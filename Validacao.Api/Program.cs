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

var secretKey = builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true 
    };
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); 


builder.Services.AddHttpClient<FreteService>(client =>
{
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
builder.Services.AddScoped<ITokenService, TokenService>();

string urlSupabase = builder.Configuration["Supabase:Url"] ?? string.Empty;
string chaveSupabase = builder.Configuration["Supabase:Chave"] ?? string.Empty;

if (string.IsNullOrEmpty(urlSupabase) || string.IsNullOrEmpty(chaveSupabase))
{
    throw new InvalidOperationException("ERRO CRÍTICO: As credenciais do Supabase não foram encontradas no appsettings.json da API!");
}

var opcoes = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Supabase.Client(urlSupabase, chaveSupabase, opcoes));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("LiberarBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/", () => "Minha API de CEP está online e organizada! 🚀");

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