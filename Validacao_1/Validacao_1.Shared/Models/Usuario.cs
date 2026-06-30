using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table("usuarios")]
public class Usuario : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("idade")]
    public int Idade { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("senha")]
    public string Senha { get; set; } = string.Empty;

    [Column("perfil")]
    public string Perfil { get; set; } = "Usuario";
}
