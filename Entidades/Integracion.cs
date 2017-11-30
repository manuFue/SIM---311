using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Integracion
    {
        private static double h = 0.05; // Paso de Integración

        // Obtiene el TIEMPO de Secado de un Trabajo, habiendo 1 sólo Trabajo siendo secado en el Equipo de Secado.
        // A partir de un M (índice de mojado).
        public static double integrar1Trabajo(double mInicial, double corte)
        {
            double tSoporte = Math.Round(0.00, 2);

            if (mInicial != 100)
            {
                tSoporte = integrar1HastaTiempo(mInicial);
            }

            double t = tSoporte;
            double M = mInicial;
            double tiempo = Math.Round(0.00, 2);

            while (M > corte)
            {
                double ecuacion = ((-0.05) * M) - (0.0001 * t);

                double MProx = M + (h * ecuacion);
                tiempo += h;
                tiempo = Math.Round(tiempo, 4);

                M = MProx;
                t += h;
                t = Math.Round(t, 4);
            }
            return tiempo;
        }

        // Obtiene el TIEMPO de Secado de un Trabajo, habiendo 2 Trabajos siendo secados en el Equipo de Secado.
        // A partir de un M (índice de mojado).
        public static double integrar2Trabajos(double mInicial, double corte)
        {
            double tSoporte = Math.Round(0.00, 2);

            if (mInicial != 100)
            {
                tSoporte = integrar2HastaTiempo(mInicial);
            }

            double t = tSoporte;
            double M = mInicial;
            double tiempo = Math.Round(0.00, 2);

            while (M > corte)
            {
                double ecuacion = ((-0.05) * M) + (0.04) - (0.0001 * t);
                double MProx = M + (h * ecuacion);
                tiempo += h;
                tiempo = Math.Round(tiempo, 4);

                M = MProx;
                t += h;
                t = Math.Round(t, 4);
            }
            return tiempo;
        }

        // Obtiene un M (ÍNDICE DE MOJADO) resultante, a partir de un tiempo de secado y un índice de mojado incial.
        // Habiendo estado secándose 1 sólo trabajo en el Equipo de Secado.
        public static double integrar1TrabajoHastaPorcentaje(double corte, double porcentaje)
        {
            double t = Math.Round(0.00, 2);
            double M = porcentaje;

            while (t <= Math.Round(corte,2))
            {
                
                double ecuacion = ((-0.05) * M) - (0.0001 * t);
                double MProx = M + (h * ecuacion);

                t += h;
                t = Math.Round(t, 2);
                if (t < corte)
                    M = MProx;
                else
                    break;
            }
            return M;
        }

        // Obtiene un M (ÍNDICE DE MOJADO) resultante, a partir de un tiempo de secado y un índice de mojado incial.
        // Habiendo estado secándose 2 trabajos en el Equipo de Secado.
        public static double integrar2TrabajosHastaPorcentaje(double corte, double porcentaje)
        {
            double t = Math.Round(0.00, 2);
            double M = porcentaje;

            while (t <= Math.Round(corte,2))
            {
                double ecuacion = ((-0.05) * M) + (0.04) - (0.0001 * t);
                double MProx = M + (h * ecuacion);

                t += h;
                t = Math.Round(t, 2);
                if (t < corte)
                    M = MProx;
                else
                    break;
            }
            return M;
        }

        // Obtiene el TIEMPO resultante de lograr un M (índice de mojado) estando 1 trabajo siendo secado en el Equipo de Secado.
        public static double integrar1HastaTiempo(double porcentaje)
        {
            double t = Math.Round(0.00, 2);
            double M = 100;

            while (M > (porcentaje))
            {
                double ecuacion = ((-0.05) * M) - (0.0001 * t);
                double MProx = M + (h * ecuacion);

                M = MProx;
                if (M > (porcentaje))
                    t += h;
                else
                    break;
            }
            t = Math.Round(t, 4);
            return t;
        }

        // Obtiene el TIEMPO resultante de lograr un M (índice de mojado) estando 2 trabajos siendo secados en el Equipo de Secado.
        public static double integrar2HastaTiempo(double porcentaje)
        {
            double t = Math.Round(0.00, 2);
            double M = 100;

            while (M > (porcentaje))
            {
                double ecuacion = ((-0.05) * M) + (0.04) - (0.0001 * t);
                double MProx = M + (h * ecuacion);

                M = MProx;
                if (M > (porcentaje))
                    t += h;
                else
                    break;
            }
            t = Math.Round(t, 4);
            return t;
        }
    }
}
