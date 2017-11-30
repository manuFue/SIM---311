using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    // Todas las distribuciones devuelven una lista de números, porque está pensado para que puedan generar más de uno a la vez.
    public class Distribuciones
    {
        public enum _Distribucion
        {
            Uniforme, Normal, Exponencial
        }

        public static Random rnd = new Random();

        public static List<Numero> Uniforme(double a, double b, double random)
        {
            List<Numero> numeros = new List<Numero>();
            double x = 0;
            x = a + (b - a) * random;
            numeros.Add(new Numero(1, x));
            return numeros;
        }

        public static List<Numero> Exponencial(double lambda, double random)
        {
            List<Numero> numeros = new List<Numero>();
            double x = 0;
            x = (-1 / lambda) * Math.Log(1 - random);
            numeros.Add(new Numero(1, x));
            return numeros;
        }

        public static List<Numero> Normal(double mu, double sigma)
        {
            List<Numero> numeros = new List<Numero>();

            double x1 = 0;
            double x2 = 0;
            double rnd1 = rnd.NextDouble();
            double rnd2 = rnd.NextDouble();

            x1 = Math.Sqrt(-2 * Math.Log(rnd1)) * Math.Cos(2 * Math.PI * rnd2) * sigma + mu;

            numeros.Add(new Numero(1, Math.Abs(x1)));

            x2 = Math.Sqrt(-2 * Math.Log(rnd1)) * Math.Sin(2 * Math.PI * rnd2) * sigma + mu;

            numeros.Add(new Numero(1, rnd1));
            numeros.Add(new Numero(2, rnd2));

            return numeros;
        }
    }
}

