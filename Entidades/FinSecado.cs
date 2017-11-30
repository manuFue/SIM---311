using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FinSecado : Evento
    {
        private double tiempoHastaFin = 0.00;
        private double horaFin = 0.00;
        private string nombreEvento = "Fin_Secado";
        private double porcentajeMojado = 0.00;
        private EquipoSecado equipoSecado;
        private int nroOrden;

        public FinSecado(EquipoSecado equipo, int nro)
        {
            this.equipoSecado = equipo;
            this.nroOrden = nro;
        }

        public EquipoSecado getEquipo()
        {
            return equipoSecado;
        }

        public int getNroOrden()
        {
            return nroOrden;
        }

        public double getPorcentajeMojado()
        {
            return porcentajeMojado;
        }

        public void setTiempoHastaFin(double tiempo)
        {
            this.tiempoHastaFin = tiempo;
        }

        public void setPorcentajeMojado(double porcentaje)
        {
            this.porcentajeMojado = porcentaje;
        }

        public void setHoraFin(double tiempo)
        {
            horaFin = tiempo;
        }

        public double getTiempoHastaFin()
        {
            return tiempoHastaFin;
        }

        public override string getNombreEvento()
        {
            return nombreEvento;
        }

        public override double getProximaLlegada()
        {
            return horaFin;
        }

        // Calcula el TIEMPO que va a durar el Secado dado que sólo hay 1 Trabajo secándose en el Equipo de Secado.
        // Calcula la Hora de Reloj final en la que se producirá el Evento FinSecado.
        public override void simular(double reloj)
        {
            tiempoHastaFin = Integracion.integrar1Trabajo(this.porcentajeMojado, 1);
            horaFin = tiempoHastaFin + reloj;
        }

        // Calcula el TIEMPO que va a durar el Secado dado que hayan 2 Trabajos secándose en el Equipo de Secado.
        // Calcula la Hora de Reloj final en la que se producirá el Evento FinSecado.
        public void simular2Trabajos(double reloj)
        {
            tiempoHastaFin = Integracion.integrar2Trabajos(this.porcentajeMojado, 1);
            horaFin = tiempoHastaFin + reloj;
        }

        // Actualiza el Índice de Mojado (Porcentaje) del trabajo según el tiempo que estuvo secándose.
        // Dado que haya habido sólo 1 Trabajo secándose en el Equipo de Secado.
        public void actualizarSecadoA1Trabajo(double reloj)
        {
            double tiempoSecandose = reloj - (horaFin - tiempoHastaFin);
            setPorcentajeMojado(Integracion.integrar1TrabajoHastaPorcentaje(tiempoSecandose, porcentajeMojado));
        }

        // Actualiza el Índice de Mojado (Porcentaje) del trabajo según el tiempo que estuvo secándose.
        // Dado que haya habido 2 Trabajos secándose en el Equipo de Secado.
        public void actualizarSecadoA2Trabajos(double reloj)
        {
            double tiempoSecandose = reloj - (horaFin - tiempoHastaFin);
            setPorcentajeMojado(Integracion.integrar2TrabajosHastaPorcentaje(tiempoSecandose, porcentajeMojado));
        }
    }
}
