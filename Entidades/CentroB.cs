using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CentroB:Centro
    {
        private _EstadosCentros estado;
        private Queue<Trabajo> cola_CentroB = new Queue<Trabajo>();
        private Trabajo siendoProcesado;

        public _EstadosCentros Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public Trabajo getSiendoProcesado()
        {
            return siendoProcesado;
        }

        public void setSiendoProcesado(Trabajo trabajo)
        {
            siendoProcesado = trabajo;
        }
        
        public Queue<Trabajo> getCola()
        {
            return cola_CentroB;
        }

        public override int tamañoCola()
        {
            return cola_CentroB.Count;
        }

        public override void ponerEnCola(Trabajo trabajo)
        {
            cola_CentroB.Enqueue(trabajo);
        }

        public override Trabajo sacarDeCola()
        {
            return cola_CentroB.Dequeue();
        }
    }
}
