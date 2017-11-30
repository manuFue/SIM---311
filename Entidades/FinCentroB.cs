using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FinCentroB:Evento
    {

        private string nombreEvento = "Fin_CentroB";
        private List<double> rndUsados = new List<double>();
        private double tiempoProceso;
        private double horaFin;
        private double media;
        private double dispersion;

        public FinCentroB(double num1, double num2)
        {
            media = num1;
            dispersion = num2;
        }

        public List<double> getRandomUsados()
        {
            return rndUsados;
        }

        public double getTiempoProceso()
        {
            return tiempoProceso;
        }

        public void setReinicio()
        {
            rndUsados.Clear();
            tiempoProceso = 0.00;
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

        // Calcula la Hora de Reloj final en la que se producirá el Evento FinCentroB.
        public override void simular(double reloj)
        {
            List<Numero> generados = Distribuciones.Normal(media, dispersion);
            tiempoProceso = generados[0].Nro;
            rndUsados.Add(generados[1].Nro);
            rndUsados.Add(generados[2].Nro);
            horaFin = tiempoProceso + reloj;
        }
    }
}
