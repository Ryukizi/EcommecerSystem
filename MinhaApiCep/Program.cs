using MinhaApiCep.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. ADICIONE ISSO: Configura a política de CORS para liberar qualquer site (inclusive seu Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7292")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registramos o HttpClient e o nosso novo Serviço no sistema de Injeção de Dependência
builder.Services.AddHttpClient<FreteService>();

var app = builder.Build();

app.UseCors("LiberarBlazor"); // 2. ADICIONE ISSO: Aplica a política de CORS)

app.MapGet("/", () => "Minha API de CEP está online e organizada! 🚀");

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

app.Run();