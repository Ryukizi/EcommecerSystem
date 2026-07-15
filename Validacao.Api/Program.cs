using Supabase;
using Validacao.Aplication.Service;
using Validacao.Application.Interfaces;
using Validacao.Infrastructure.Services;

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

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<FreteService>();

// 💡 Registra o serviço de aplicação de produtos
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// 💡 Registra o serviço de criptografia para que a API consiga usá-lo nos controllers
builder.Services.AddScoped<ICriptografiaServico, CriptografiaServico>();
// Adicione o registro do seu validador (use o escopo adequado, geralmente Scoped ou Transient)
builder.Services.AddScoped<IValidadorService, ValidadorService>();

builder.Services.AddScoped<IEmailService, EmailServico>();

builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();

// Diz ao sistema: "Quando alguém pedir IUsuarioRepository, entrega o UsuarioRepository da Infra"
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

string urlSupabase = builder.Configuration["Supabase:Url"] ?? string.Empty;
string chaveSupabase = builder.Configuration["Supabase:Chave"] ?? string.Empty;

var baseUrl = builder.Configuration["Smtp:Senha"] ?? string.Empty;

// 💡 Validação para garantir que as chaves não estão vindo vazias do appsettings.json
if (string.IsNullOrEmpty(urlSupabase) || string.IsNullOrEmpty(chaveSupabase))
{
    throw new InvalidOperationException("ERRO CRÍTICO: As credenciais do Supabase não foram encontradas no appsettings.json da API!");
}

var opcoes = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Supabase.Client(urlSupabase, chaveSupabase, opcoes));


var app = builder.Build();

app.UseCors("LiberarBlazor"); // 2. ADICIONE ISSO: Aplica a política de CORS)

app.MapGet("/", () => "Minha API de CEP está online e organizada! 🚀");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// A rota agora apenas chama o serviço e decide o que responder (Foco total em HTTP)
app.MapGet("/cep/{numeroCep}", async (string numeroCep, FreteService freteService) =>
{
    var resultado = await freteService.CalcularFreteAsync(numeroCep);

    if (resultado == null)
    {
        return Results.NotFound(new { mensagem = "CEP não encontrado ou inválido." });
    }

    return Results.Ok(resultado);
});

// Garanta que esta linha também existe!
app.MapControllers();

app.Run();
