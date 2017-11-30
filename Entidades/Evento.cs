using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public abstract class Evento : IComparable<Evento>
    {
        public abstract double getProximaLlegada();
        public abstract string getNombreEvento();
        public abstract void simular(double reloj);

        // Permite que cada evento se compare a otro según su Atributo "próximaLlegada".
        public int CompareTo(Evento otroEvento)
        {
            return Math.Sign(this.getProximaLlegada() - otroEvento.getProximaLlegada());
        }

        // Obtiene el siguiente EVENTO a ocurrir según el Atributo "próximaLlegada" de cada uno de los eventos, comparándolos.
        public Evento getSiguienteEvento(Evento otroEvento)
        {
            if (getProximaLlegada() == 0.00)
            {
                return otroEvento;
            }
            else if (otroEvento == null || otroEvento.getProximaLlegada() == 0.00)
            {
                return this;
            }
            else if (this.getProximaLlegada() < otroEvento.getProximaLlegada())
            {
                return this;
            }
            else
            {
                return otroEvento;
            }

        }
    }
}

