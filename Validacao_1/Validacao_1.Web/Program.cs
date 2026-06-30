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