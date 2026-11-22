using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("guitarras")]
public class Guitarras : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("preco")]
    public decimal Preco { get; set; }

    [Column("marca")]
    public string Marca { get; set; } = string.Empty;

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("url_imagem")]
    public string Url_imagem { get; set; } = string.Empty;
}