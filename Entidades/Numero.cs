using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Numero
    {
        private int orden;
        private double numero;

        public Numero(int ord, double nro)
        {
            orden = ord;
            this.numero = nro;
        }

        public int Orden
        {
            get { return orden; }
            set { orden = value; }
        }

        public double Nro
        {
            get { return numero; }
            set { numero = value; }
        }
    }
}
