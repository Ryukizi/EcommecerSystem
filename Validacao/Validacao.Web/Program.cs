using Blazor.Extensions.Storage;
using MailKit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase;
using System;
using System.Net.Http;
using Validacao.Application.Interfaces;
using Validacao.Infrastructure.Services;
using Validacao.Web;
using Validacao.Web.Components;
using Validacao.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Storage e autenticação do Blazor
builder.Services.AddStorage();

// Registra a classe concreta primeiro
builder.Services.AddScoped<SupaBaseAuthStateProviderService>();

// Registra o AuthenticationStateProvider nativo do Blazor apontando para a sua classe concreta
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<SupaBaseAuthStateProviderService>());

builder.Services.AddAuthenticationCore();

// 2. 🔑 RESOLVENDO O CONFLITO DE NAMESPACE: 
// Forçamos explicitamente o uso da SUA interface de Application para evitar conflitos com o SDK do Supabase
builder.Services.AddScoped<Validacao.Application.Interfaces.IAuthStateProvider>(sp =>
    sp.GetRequiredService<SupaBaseAuthStateProviderService>());


// 4. 🛡️ O ÚNICO E DEFINITIVO HTTPCLIENT DA APLICAÇÃO
builder.Services.AddScoped(sp =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7021/";

    if (!baseUrl.EndsWith("/"))
    {
        baseUrl += "/";
    }

    var handler = new HttpClientHandler
    {
        // Ignora erros de certificado SSL inválido comuns no desenvolvimento local
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };
});


// 5. Registrar Componentes Razor do Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// 6. Registro de Serviços de Negócio (Injeção de Dependência)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidadorService, ValidadorService>();
builder.Services.AddScoped<CriptografiaServico>();
builder.Services.AddTransient<IEmailService, EmailServico>();
builder.Services.AddScoped<CarrinhoService>();

// Registros de Serviços de Produto (Limpo e sem duplicidade)
builder.Services.AddScoped<Validacao.Application.Interfaces.IProdutoService, Validacao.Infrastructure.Services.ProdutoService>();


var app = builder.Build();


// 7. Pipeline de execução do HTTP (Middlewares)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();