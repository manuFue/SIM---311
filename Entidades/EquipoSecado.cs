using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class EquipoSecado
    {
        private int idEquipo;
        private _EstadosEquiposSecado estado;
        private Trabajo lugar1_siendoSecado;
        private Trabajo lugar2_siendoSecado;

        public EquipoSecado(int i)
        {
            idEquipo = i;
            estado = _EstadosEquiposSecado.Libre;
        }

        public int ID
        {
            get { return idEquipo; }
            set { idEquipo = value; }
        }

        public _EstadosEquiposSecado Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public Trabajo Lugar1
        {
            get { return lugar1_siendoSecado; }
            set { lugar1_siendoSecado = value; }
        }

        public Trabajo Lugar2
        {
            get { return lugar2_siendoSecado; }
            set { lugar2_siendoSecado = value; }
        }
    }
}
