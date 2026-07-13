using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase;

using Validacao_1.Shared.Services;
using Validacao_1.Web.Components;
using Validacao_1.Web.Services;           

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStorage();

builder.Services.AddAuthenticationCore();

builder.Services.AddScoped<SupabaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<SupabaseAuthStateProvider>());

string urlSupabase = builder.Configuration["Supabase:Url"] ?? string.Empty;
string chaveSupabase = builder.Configuration["Supabase:Chave"] ?? string.Empty;


var opcoes = new SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Client(urlSupabase, chaveSupabase, opcoes));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IValidador, Validador>();

builder.Services.AddScoped<Validacao_1.Shared.Services.CriptografiaServico>();

builder.Services.AddScoped<Validacao_1.Shared.Services.EmailServico>();

builder.Services.AddScoped<Validacao_1.Shared.Services.ProdutoService>();

builder.Services.AddScoped<Validacao_1.Shared.Services.CarrinhoService>();

// Configura o HttpClient Factory para aceitar o certificado inválido do localhost
builder.Services.AddHttpClient("ApiLocal", client =>
{
    // Define um timeout seguro para não travar a aplicação caso a API suma
    client.Timeout = TimeSpan.FromSeconds(5); 
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

// Registra o HttpClient padrão usando a configuração acima
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiLocal"));

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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Validacao_1.Shared.Pages.CadastroForm).Assembly);

app.Run();