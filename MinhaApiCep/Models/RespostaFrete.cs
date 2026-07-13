namespace MinhaApiCep.Models;

public class RespostaFrete
{
    public string CepBuscado { get; set; } = "";
    public string Cidade { get; set; } = "";
    public string Estado { get; set; } = "";
    public decimal ValorPac { get; set; }
    public string PrazoPac { get; set; } = "";
    public decimal ValorSedex { get; set; }
    public string PrazoSedex { get; set; } = "";
}
