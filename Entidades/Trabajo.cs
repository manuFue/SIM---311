using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Trabajo
    {
        private string id;
        private _EstadosTrabajo estado;
        private double horaInicioTrabajo;
        private string lugarSecado;

        public Trabajo(string id, _EstadosTrabajo estado, double horaInicioTrabajo)
        {
            this.id = id;
            this.estado = estado;
            this.horaInicioTrabajo = horaInicioTrabajo;
        }

        public void setHoraInicioTrabajo(double horaInicioTrabajo)
        {
            this.horaInicioTrabajo = horaInicioTrabajo;
        }

        public double getHoraInicioTrabajo()
        {
            return horaInicioTrabajo;
        }

        public void setLugarSecado(string lugarDeSecado)
        {
            this.lugarSecado = lugarDeSecado;
        }

        public string getLugarSecado()
        {
            return lugarSecado;
        }

        public _EstadosTrabajo Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
