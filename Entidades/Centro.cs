using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public abstract class Centro
    {
        public abstract int tamañoCola();
        public abstract void ponerEnCola(Trabajo trabajo);
        public abstract Trabajo sacarDeCola();
    }
}
