using Blazor.Extensions.Storage;
using MailKit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Supabase;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using Validacao.Application.Interfaces;
using Validacao.Infrastructure.Services;
using Validacao.Web;
using Validacao.Web.Components;
using Validacao.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStorage();

builder.Services.AddScoped<SupaBaseAuthStateProviderService>();

builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<SupaBaseAuthStateProviderService>());

builder.Services.AddAuthenticationCore();

builder.Services.AddScoped<Validacao.Application.Interfaces.IAuthStateProvider>(sp =>
    sp.GetRequiredService<SupaBaseAuthStateProviderService>());



builder.Services.AddScoped(sp =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7021/";

    if (!baseUrl.EndsWith("/"))
    {
        baseUrl += "/";
    }

    var sslhandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    var refreshHttpClient = new HttpClient(sslhandler)
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };

    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    var jwtHandler = new JwtAuthorizationHandler(jsRuntime, refreshHttpClient)
    {
        InnerHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        }
    };

    return new HttpClient(jwtHandler)
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };
});


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidadorService, ValidadorService>();
builder.Services.AddScoped<CriptografiaServico>();
builder.Services.AddTransient<IEmailService, EmailServico>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<IFreteService, FreteService>();


builder.Services.AddScoped<Validacao.Application.Interfaces.IProdutoService, Validacao.Infrastructure.Services.ProdutoService>();

var app = builder.Build();


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
