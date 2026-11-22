using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("tabelafrete")]
public class Frete : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("uf")]
    public string Uf { get; set; } = string.Empty;

    [Column("valorpac")]
    public decimal ValorPac { get; set; }

    [Column("prazopac")]
    public string PrazoPac { get; set; } = string.Empty;

    [Column("valorsedex")]
    public decimal ValorSedex { get; set; }

    [Column("prazosedex")]
    public string PrazoSedex { get; set; } = string.Empty;
}