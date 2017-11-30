using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FinCentroA : Evento
    {
        private string nombreEvento = "Fin_CentroA";
        private double rndTiempo;
        private double tiempoProceso;
        private double horaFin;
        private double uniformeInicio;
        private double uniformeFin;
        private Random random = new Random();

        public FinCentroA(double num1, double num2)
        {
            uniformeInicio = num1;
            uniformeFin = num2;
        }

        public double getRandomUsado()
        {
            return rndTiempo;
        }

        public double getTiempoProceso()
        {
            return tiempoProceso;
        }

        public void setHoraFin(double tiempo)
        {
            horaFin = tiempo;
        }

        public override double getProximaLlegada()
        {
            return horaFin;
        }

        public override string getNombreEvento()
        {
            return nombreEvento;
        }

        // Calcula la Hora de Reloj final en la que se producirá el Evento FinCentroA.
        public override void simular(double reloj)
        {
            rndTiempo = random.NextDouble();
            tiempoProceso = Distribuciones.Uniforme(uniformeInicio, uniformeFin, rndTiempo)[0].Nro;
            horaFin = tiempoProceso + reloj;
        }
    }
}
