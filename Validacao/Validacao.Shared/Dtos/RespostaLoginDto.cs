using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validacao.Shared.Dtos
{
    public class RespostaLogin
    {
        public string Message { get; set; } = string.Empty;
        public UsuarioInfo? Usuario { get; set; }
    }
}
