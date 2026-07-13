using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validacao_1.Shared.Models
{
    public class RespostaApiFrete
    {
        public string CepBuscado { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public decimal ValorPac { get; set; }
        public string PrazoPac { get; set; } = "";
        public decimal ValorSedex { get; set; }
        public string PrazoSedex { get; set; } = "";
    }
}
