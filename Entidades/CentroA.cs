using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CentroA:Centro
    {
        private _EstadosCentros estado;
        private Queue<Trabajo> cola_CentroA = new Queue<Trabajo>();
        private double inicioTiempoParada;
        private Trabajo siendoProcesado;

        public _EstadosCentros Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public double InicioTiempoParada
        {
            get { return inicioTiempoParada; }
            set { inicioTiempoParada = value; }
        }

        public Queue<Trabajo> getCola()
        {
            return cola_CentroA;
        }

        public override int tamañoCola()
        {
            return cola_CentroA.Count;
        }

        public override void ponerEnCola(Trabajo trabajo)
        {
            cola_CentroA.Enqueue(trabajo);
        }

        public override Trabajo sacarDeCola()
        {
            return cola_CentroA.Dequeue();
        }

        public Trabajo getSiendoProcesado()
        {
            return siendoProcesado;
        }

        public void setSiendoProcesado(Trabajo trabajo)
        {
            siendoProcesado = trabajo;
        }
    }
}
