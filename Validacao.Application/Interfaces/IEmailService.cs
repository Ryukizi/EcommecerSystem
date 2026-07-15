using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validacao.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailCodigo(string emailDoCliente, int codigo);
    }
}
