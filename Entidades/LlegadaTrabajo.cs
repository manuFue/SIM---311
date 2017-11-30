using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class LlegadaTrabajo : Evento
    {
        private double rndTiempo;
        private double tiempoEntreLlegadas;
        private double proximaLlegada;
        private string nombreEvento = "Llegada_Trabajo";
        private Random random = new Random();
        private double lambda;

        public LlegadaTrabajo(double lambda)
        {
            this.lambda = lambda;
        }

        public override double getProximaLlegada()
        {
            return proximaLlegada;
        }

        public override string getNombreEvento()
        {
            return nombreEvento;
        }
        
        public double getRandom()
        {
            return rndTiempo;
        }

        public double getTiempoEntreLlegada()
        {
            return tiempoEntreLlegadas;
        }

        // Calcula la Hora de Reloj final en la que se producirá el Evento LlegadaTrabajo.
        public override void simular(double reloj)
        {
            rndTiempo = random.NextDouble();
            tiempoEntreLlegadas = Distribuciones.Exponencial(lambda, rndTiempo)[0].Nro;
            proximaLlegada = tiempoEntreLlegadas + reloj;
        }
    }
}
