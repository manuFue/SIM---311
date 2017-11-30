using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public enum _EstadosCentros
    {
        Libre,
        Ocupado,
        Detenido
    }
    public enum _EstadosTrabajo
    {
        Siendo_Procesado_CentroA,
        Siendo_Procesado_CentroB,
        Esperando_CentroA,
        Esperando_CentroB,
        Siendo_Secado
    }
    public enum _EstadosEquiposSecado
    {
        Libre,
        Semi_Ocupado,
        Ocupado_Total
    }
    public enum _Origen
    {
        llegadaTrabajo,
        finCentroA,
        finCentroB,
        finSecado
    }
}
