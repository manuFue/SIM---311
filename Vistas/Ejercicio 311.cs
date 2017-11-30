using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;

namespace Vistas
{
    public partial class Form1 : Form
    {
        TextBox invalido = null;

        private double relojCentral;
        private int cantidadEventosSimulados;
        private double tiempoFinSimulacion = 0.00;
        private int eventosFinSimulacion = 0;
        private int cantMaxTrabajos;
        private int trabajosActivosAhora;
        private double tiempoTotalParadaCentroA;
        private int cantidadTrabajosTerminados;
        private double acumuladoTrabajosTerminados;
        private int identificadorTrabajo;
        private double limiteTiempoMenor = 0;
        private double limiteTiempoMayor = 0;
        private int limiteEventoMenor = 0;
        private int limiteEventoMayor = 0;
        private bool simuloCentroA = false;
        private bool simuloCentroB = false;
        private bool bandera_AgregadosActivos = false;

        private LlegadaTrabajo llegada_Trabajo;
        private FinCentroA fin_CentroA;
        private FinCentroB fin_CentroB;
        private CentroA centro_A;
        private CentroB centro_B;
        private List<EquipoSecado> equiposDeSecado = new List<EquipoSecado>();
        private List<Trabajo> siendoSecados = new List<Trabajo>();

        private List<FinSecado> EventosSecadoPosicionesPrincipales = new List<FinSecado>();
        private List<FinSecado> EventosSecadoPosicionesSecundarias = new List<FinSecado>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Calcula la relación sugerida sobre a partir de qué tiempo mostrar en tabla los eventos, según lo ingresado en el TextBox "TiempoSimulacion".
        private void txtTiempoSimulacion_TextChanged(object sender, EventArgs e)
        {
            double validar;
            if ((txt_TiempoSimulacion.Text.Equals(String.Empty) || !(Double.TryParse(txt_TiempoSimulacion.Text, out validar))))
            {
                txt_tiempoAPartir.Text = "####";
                txt_tiempoHasta.Text = "####";
            }
            else
            {
                double tiempoSim = Convert.ToDouble(txt_TiempoSimulacion.Text);
                txt_tiempoHasta.Text = tiempoSim.ToString();
                txt_tiempoAPartir.Text = (tiempoSim - (tiempoSim / 5)).ToString();
            }
        }

        // Calcula la relación sugerida sobre a partir de qué evento mostrar en tabla los eventos, según lo ingresado en el TextBox "EventosASimular".
        // Redondea el número a un entero número de eventos.
        private void txt_eventosASimular_TextChanged(object sender, EventArgs e)
        {
            double validar;
            if ((txt_eventosASimular.Text.Equals(String.Empty) || !(Double.TryParse(txt_eventosASimular.Text, out validar))))
            {
                txt_eventoAPartir.Text = "####";
                txt_eventoHasta.Text = "####";
            }
            else
            {
                double cantidadEventos = Convert.ToDouble(txt_eventosASimular.Text);
                txt_eventoAPartir.Text = Math.Round((cantidadEventos - (cantidadEventos / 5))).ToString();
                txt_eventoHasta.Text = cantidadEventos.ToString();
            }
        }

        // Calcula el Lambda correspondiente (Cantidad de Trabajos por Minuto) en relación a lo ingresado en el TextBox "LlegadaTrabajo".
        private void txtLlegadaTrabajo_TextChanged(object sender, EventArgs e)
        {
            double validar;
            if (Double.TryParse(txtLlegadaTrabajo.Text, out validar))
                txtLambda.Text = Math.Round(((Convert.ToDouble(txtLlegadaTrabajo.Text) / 60)), 3).ToString();
            else
                txtLambda.Text = "####";
        }

        // Reinicia la selección de las alternativas para simular (Por Tiempo o Eventos).
        private void reiniciarRadioButtons()
        {
            txt_TiempoSimulacion.Enabled = true;
            txt_tiempoAPartir.Enabled = true;
            txt_tiempoHasta.Enabled = true;
            txt_eventosASimular.Enabled = false;
            txt_eventoAPartir.Enabled = false;
            txt_eventoHasta.Enabled = false;
            txt_TiempoSimulacion.Text = "200";
            txt_tiempoAPartir.Text = "180";
            txt_tiempoHasta.Text = "200";
            txt_eventosASimular.Text = String.Empty;
            txt_eventoAPartir.Text = String.Empty;
            txt_eventoHasta.Text = String.Empty;
        }

        // Habilita los controles para ingresar los parámetros para la simulación por TIEMPO.
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            reiniciarRadioButtons();
        }

        // Habilita los controles para ingresar los parámetros para la simulación por EVENTOS.
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txt_eventosASimular.Enabled = true;
            txt_eventoHasta.Enabled = true;
            txt_eventoAPartir.Enabled = true;
            txt_TiempoSimulacion.Enabled = false;
            txt_tiempoAPartir.Enabled = false;
            txt_tiempoHasta.Enabled = false;
            txt_eventosASimular.Text = "100";
            txt_eventoAPartir.Text = "80";
            txt_eventoHasta.Text = "100";
            txt_TiempoSimulacion.Text = String.Empty;
            txt_tiempoHasta.Text = String.Empty;
            txt_tiempoAPartir.Text = String.Empty;
        }

        // Valida los datos de todos los TextBox de la pantalla: que no estén en blanco o tengan un valor incorrecto.
        private bool validarDatos()
        {
            bool resul = true;
            double validar2;
            foreach (GroupBox grupo in this.Controls.OfType<GroupBox>())
            {
                foreach (TextBox textBoxx in grupo.Controls.OfType<TextBox>())
                {
                    if ((textBoxx.Text.Equals(String.Empty) || !(Double.TryParse(textBoxx.Text, out validar2))) && (textBoxx.Enabled == true))
                    {
                        resul = false;
                        invalido = textBoxx;
                        break;
                    }
                }
                if (resul == false)
                    break;
            }

            return resul;
        }

        // Valida que los parámetros de simulación estén correctos y sean coherentes entre si.
        private bool validarIntervalos()
        {
            bool resul = true;

            if (radiob_porEventos.Checked == true)
            {
                if ((int)Math.Round(Convert.ToDouble(txt_eventoAPartir.Text)) > Convert.ToInt32(txt_eventoHasta.Text))
                {
                    resul = false;
                    invalido = txt_eventoAPartir;
                }
                if (txt_eventosASimular.Text == "0")
                {
                    resul = false;
                    invalido = txt_eventosASimular;
                }

                if (txt_eventoAPartir.Text == "0")
                {
                    resul = false;
                    invalido = txt_eventoAPartir;
                }
            }
            else
            {
                if (Convert.ToDouble(txt_tiempoAPartir.Text) > Convert.ToDouble(txt_tiempoHasta.Text))
                {
                    resul = false;
                    invalido = txt_tiempoAPartir;
                }

                if (txt_TiempoSimulacion.Text == "0")
                {
                    resul = false;
                    invalido = txt_TiempoSimulacion;
                }
            }
            return resul;
        }

        // Vacía la tabla de resultados. Elimina las columnas extras agregadas en una simulación previa si las hubiese.
        private void reiniciarTabla()
        {
            dgv_Simulacion.Rows.Clear();
            if (dgv_Simulacion.Columns.Count > 56)
            {

                int columnasExtras = dgv_Simulacion.Columns.Count - 56;
                int ultimoTrabajo = Convert.ToInt32(dgv_Simulacion.Columns[56].Name.Split('_')[1]);
                if (columnasExtras > 0)
                {
                    for (int i = ultimoTrabajo; i <= (ultimoTrabajo + (columnasExtras / 2)); i++)
                    {
                        try
                        {
                            dgv_Simulacion.Columns.Remove("estadoTrabajo_" + i);
                            dgv_Simulacion.Columns.Remove("horaInicioTrabajo" + i);
                        }
                        catch (Exception)
                        { }
                    }
                }
            }
        }

        // Resetea variables utilizadas en la simulación.
        private void reiniciarValores()
        {
            equiposDeSecado.Clear();
            siendoSecados.Clear();
            EventosSecadoPosicionesPrincipales.Clear();
            EventosSecadoPosicionesSecundarias.Clear();
            cantidadEventosSimulados = 0;
            limiteTiempoMenor = 0.00;
            limiteTiempoMayor = 0.00;
            limiteEventoMayor = 0;
            limiteEventoMenor = 0;
            cantMaxTrabajos = 0;
            trabajosActivosAhora = 0;
            tiempoTotalParadaCentroA = 0.00;
            cantidadTrabajosTerminados = 0;
            acumuladoTrabajosTerminados = 0.00;
            identificadorTrabajo = 0;
            simuloCentroA = false;
            simuloCentroB = false;
            bandera_AgregadosActivos = false;
        }

        // Vuelve a los valores por defectos los TextBox de la pantalla.
        private void reiniciarPantalla()
        {
            txtLlegadaTrabajo.Text = "5";
            txtLambda.Text = Math.Round(((Convert.ToDouble(txtLlegadaTrabajo.Text) / 60)), 3).ToString();
            txt_ProcesoA_1.Text = "1";
            txt_ProcesoA_2.Text = "10";
            txt_ProcesoB_Desviacion.Text = "5";
            txt_ProcesoB_Media.Text = "8";
            txt_TiempoSimulacion.Text = "200";
            txt_tiempoAPartir.Text = "160";
            txt_tiempoHasta.Text = "200";
            txtMaximoTrabajosActivos.Text = "0";
            txtTiempoParadoCentroA.Text = "0.00";
            txtTiempoPromedioTrabajo.Text = "0.00";
        }

        private void btn_Reiniciar_Click(object sender, EventArgs e)
        {
            GC.Collect();
            reiniciarPantalla();
            reiniciarValores();
            reiniciarTabla();
            reiniciarRadioButtons();
            radiob_porTiempo.Checked = true;
        }

        private void btn_simular_Click(object sender, EventArgs e)
        {
            GC.Collect();
            if (validarDatos() && validarIntervalos())
            {
                this.Cursor = Cursors.WaitCursor;
                reiniciarValores();
                reiniciarTabla();
                relojCentral = 0.00;
                if (radiob_porTiempo.Checked == true)
                {
                    tiempoFinSimulacion = Convert.ToDouble(txt_TiempoSimulacion.Text);
                    limiteTiempoMenor = Convert.ToDouble(txt_tiempoAPartir.Text);
                    limiteTiempoMayor = Convert.ToDouble(txt_tiempoHasta.Text);
                }
                if (radiob_porEventos.Checked == true)
                {
                    eventosFinSimulacion = Convert.ToInt32(txt_eventosASimular.Text);
                    limiteEventoMenor = (int)Math.Round(Convert.ToDouble(txt_eventoAPartir.Text));
                    limiteEventoMayor = Convert.ToInt32(txt_eventoHasta.Text);
                }
                double lambda = Convert.ToDouble(txtLambda.Text);
                llegada_Trabajo = new LlegadaTrabajo(lambda);
                fin_CentroA = new FinCentroA(Int32.Parse(txt_ProcesoA_1.Text), Int32.Parse(txt_ProcesoA_2.Text));
                fin_CentroB = new FinCentroB(Int32.Parse(txt_ProcesoB_Media.Text), Int32.Parse(txt_ProcesoB_Desviacion.Text));
                centro_A = new CentroA();
                centro_B = new CentroB();
                EquipoSecado equipo1 = new EquipoSecado(1);
                EquipoSecado equipo2 = new EquipoSecado(2);
                EquipoSecado equipo3 = new EquipoSecado(3);
                EquipoSecado equipo4 = new EquipoSecado(4);
                EquipoSecado equipo5 = new EquipoSecado(5);
                equiposDeSecado.Add(equipo1);
                equiposDeSecado.Add(equipo2);
                equiposDeSecado.Add(equipo3);
                equiposDeSecado.Add(equipo4);
                equiposDeSecado.Add(equipo5);


                FinSecado finSecado_Eq1_1 = new FinSecado(equipo1, 1);
                FinSecado finSecado_Eq1_2 = new FinSecado(equipo1, 2);
                FinSecado finSecado_Eq2_1 = new FinSecado(equipo2, 1);
                FinSecado finSecado_Eq2_2 = new FinSecado(equipo2, 2);
                FinSecado finSecado_Eq3_1 = new FinSecado(equipo3, 1);
                FinSecado finSecado_Eq3_2 = new FinSecado(equipo3, 2);
                FinSecado finSecado_Eq4_1 = new FinSecado(equipo4, 1);
                FinSecado finSecado_Eq4_2 = new FinSecado(equipo4, 2);
                FinSecado finSecado_Eq5_1 = new FinSecado(equipo5, 1);
                FinSecado finSecado_Eq5_2 = new FinSecado(equipo5, 2);
                EventosSecadoPosicionesPrincipales.Add(finSecado_Eq1_1);
                EventosSecadoPosicionesPrincipales.Add(finSecado_Eq2_1);
                EventosSecadoPosicionesPrincipales.Add(finSecado_Eq3_1);
                EventosSecadoPosicionesPrincipales.Add(finSecado_Eq4_1);
                EventosSecadoPosicionesPrincipales.Add(finSecado_Eq5_1);

                EventosSecadoPosicionesSecundarias.Add(finSecado_Eq1_2);
                EventosSecadoPosicionesSecundarias.Add(finSecado_Eq2_2);
                EventosSecadoPosicionesSecundarias.Add(finSecado_Eq3_2);
                EventosSecadoPosicionesSecundarias.Add(finSecado_Eq4_2);
                EventosSecadoPosicionesSecundarias.Add(finSecado_Eq5_2);

                // -------- Inicio de Simulación ----------

                if (relojCentral == 0.00)
                {
                    llegada_Trabajo.simular(relojCentral);
                    int i = dgv_Simulacion.Rows.Add();
                    dgv_Simulacion.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    dgv_Simulacion.Rows[i].Cells["evento"].Value = "Inicio Simulación";
                    dgv_Simulacion.Rows[i].Cells["relojGrilla"].Value = relojCentral;
                    agregarProximoTrabajoAGrilla(i, true);
                    dgv_Simulacion.Rows[i].Cells["proxLlegada"].Style.BackColor = Color.Cyan;
                    dgv_Simulacion.Rows[i].Cells["rndCentroA"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoProcesoA"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finCentroA"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["rnd1CentroB"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["rnd2CentroB"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoProcesoB"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finCentroB"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq1_1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq1_2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq2_1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq2_2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq3_1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq3"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo3"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq3_2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq3"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo3"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq4_1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq4"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo4"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq4_2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq4"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo4"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq5_1"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq5"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo5"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq5_2"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq5"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo5"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["estadoCentroA"].Value = centro_A.Estado;
                    dgv_Simulacion.Rows[i].Cells["inicioTiempoParada"].Value = "-";
                    dgv_Simulacion.Rows[i].Cells["colaCentroA"].Value = centro_A.tamañoCola();
                    dgv_Simulacion.Rows[i].Cells["estadoCentroB"].Value = centro_B.Estado;
                    dgv_Simulacion.Rows[i].Cells["colaCentroB"].Value = centro_B.tamañoCola();
                    dgv_Simulacion.Rows[i].Cells["maxTrabajosActivos"].Value = cantMaxTrabajos;
                    dgv_Simulacion.Rows[i].Cells["tiempoParadaA"].Value = tiempoTotalParadaCentroA;
                    dgv_Simulacion.Rows[i].Cells["trabajosTerminados"].Value = cantidadTrabajosTerminados;
                    dgv_Simulacion.Rows[i].Cells["tiempoTrabajoAcumulado"].Value = acumuladoTrabajosTerminados;
                }

                while ((relojCentral < tiempoFinSimulacion) || (cantidadEventosSimulados < eventosFinSimulacion))
                {
                    cantidadEventosSimulados++;

                    Evento siguiente = llegada_Trabajo.getSiguienteEvento(fin_CentroA.getSiguienteEvento(fin_CentroB.getSiguienteEvento(finSecado_Eq1_1.getSiguienteEvento(
                        finSecado_Eq1_2.getSiguienteEvento(finSecado_Eq2_1.getSiguienteEvento(finSecado_Eq2_2.getSiguienteEvento(finSecado_Eq3_1.getSiguienteEvento(finSecado_Eq3_2.getSiguienteEvento(
                            finSecado_Eq4_1.getSiguienteEvento(finSecado_Eq4_2.getSiguienteEvento(finSecado_Eq5_1.getSiguienteEvento(finSecado_Eq5_2))))))))))));

                    relojCentral = siguiente.getProximaLlegada();

                    // -------- Evaluar Siguiente Evento ----------

                    if (siguiente is LlegadaTrabajo)
                    {
                        double acumuladorAncho = 0;

                        for (int l = 0; l <= ((dgv_Simulacion.Columns.Count) - 1); l++)
                        {
                            acumuladorAncho += dgv_Simulacion.Columns[l].FillWeight;
                        }

                        identificadorTrabajo++;
                        if (bandera_AgregadosActivos)
                        {
                            if ((relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor) ||
                                (cantidadEventosSimulados >= limiteEventoMenor && cantidadEventosSimulados < limiteEventoMayor))
                            {
                                dgv_Simulacion.Columns.Add("estadoTrabajo_" + identificadorTrabajo, "Estado Trbj " + identificadorTrabajo);
                                dgv_Simulacion.Columns.Add("horaInicioTrabajo" + identificadorTrabajo, "Inicio Trbj " + identificadorTrabajo);
                                dgv_Simulacion.Columns["horaInicioTrabajo" + identificadorTrabajo].Width = 50;
                            }
                        }

                        if (centro_A.Estado == _EstadosCentros.Libre)
                        {
                            centro_A.Estado = _EstadosCentros.Ocupado;
                            centro_A.setSiendoProcesado(new Trabajo("estadoTrabajo_" + identificadorTrabajo, _EstadosTrabajo.Siendo_Procesado_CentroA, relojCentral));
                            fin_CentroA.simular(relojCentral);
                            simuloCentroA = true;
                        }
                        else
                            centro_A.ponerEnCola(new Trabajo("estadoTrabajo_" + identificadorTrabajo, _EstadosTrabajo.Esperando_CentroA, relojCentral));

                        llegada_Trabajo.simular(relojCentral);

                        if (!bandera_AgregadosActivos)
                        {
                            if (cantidadEventosSimulados == limiteEventoMenor || (relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor && relojCentral != 0.00))
                            {
                                agregarTrabajosActivos(centro_A.getSiendoProcesado(), centro_A.getCola(), centro_B.getSiendoProcesado(), centro_B.getCola(), siendoSecados);
                                bandera_AgregadosActivos = true;
                            }
                        }

                        if ((relojCentral > limiteTiempoMenor && relojCentral < limiteTiempoMayor) ||
                                    (cantidadEventosSimulados >= limiteEventoMenor && cantidadEventosSimulados < limiteEventoMayor))
                        {
                            int i = dgv_Simulacion.Rows.Add();
                            agregarNuevaFila(siguiente, true, centro_A.getSiendoProcesado(), centro_B.getSiendoProcesado(), siendoSecados, i);
                            if (i != 1)
                                dgv_Simulacion.Rows[i - 1].Cells["proxLlegada"].Style.BackColor = Color.Cyan;
                        }
                    }
                    else if (siguiente is FinCentroA)
                    {
                        int i = 0;
                        if (centro_B.Estado == _EstadosCentros.Libre)
                        {
                            centro_B.Estado = _EstadosCentros.Ocupado;
                            centro_B.setSiendoProcesado(new Trabajo(centro_A.getSiendoProcesado().Id, _EstadosTrabajo.Siendo_Procesado_CentroB, centro_A.getSiendoProcesado().getHoraInicioTrabajo()));
                            fin_CentroB.simular(relojCentral);
                            simuloCentroB = true;
                        }
                        else
                        {
                            centro_B.ponerEnCola(new Trabajo(centro_A.getSiendoProcesado().Id, _EstadosTrabajo.Esperando_CentroB, centro_A.getSiendoProcesado().getHoraInicioTrabajo()));
                            if (centro_B.tamañoCola() == 3)
                            {
                                centro_A.Estado = _EstadosCentros.Detenido;
                                centro_A.InicioTiempoParada = relojCentral;
                            }
                        }

                        centro_A.setSiendoProcesado(null);
                        fin_CentroA.setHoraFin(0.00);

                        if (centro_A.Estado != _EstadosCentros.Detenido)
                        {
                            if (centro_A.tamañoCola() > 0)
                            {
                                Trabajo sacadoDeCola = centro_A.sacarDeCola();
                                sacadoDeCola.Estado = _EstadosTrabajo.Siendo_Procesado_CentroA;
                                centro_A.setSiendoProcesado(sacadoDeCola);
                                fin_CentroA.simular(relojCentral);
                                simuloCentroA = true;
                            }
                            else
                                centro_A.Estado = _EstadosCentros.Libre;
                        }

                        if (!bandera_AgregadosActivos)
                        {
                            if (cantidadEventosSimulados == limiteEventoMenor || (relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor && relojCentral != 0.00))
                            {
                                agregarTrabajosActivos(centro_A.getSiendoProcesado(), centro_A.getCola(), centro_B.getSiendoProcesado(), centro_B.getCola(), siendoSecados);
                                bandera_AgregadosActivos = true;
                            }
                        }

                        if ((relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor) ||
                            (cantidadEventosSimulados >= limiteEventoMenor && cantidadEventosSimulados < limiteEventoMayor))
                        {
                            i = dgv_Simulacion.Rows.Add();
                            agregarNuevaFila(siguiente, false, centro_A.getSiendoProcesado(), centro_B.getSiendoProcesado(), siendoSecados, i);
                            if (i != 1)
                                dgv_Simulacion.Rows[i - 1].Cells["finCentroA"].Style.BackColor = Color.Cyan;
                        }

                    }
                    else if (siguiente is FinCentroB)
                    {
                        int i = 0;
                        bool equiposLibres = false;

                        foreach (EquipoSecado equipo in equiposDeSecado)
                        {
                            if (equipo.Estado == _EstadosEquiposSecado.Libre)
                            {
                                equipo.Lugar1 = centro_B.getSiendoProcesado();
                                equipo.Lugar1.setLugarSecado("Eq." + equipo.ID + "_1");
                                equipo.Lugar1.Estado = _EstadosTrabajo.Siendo_Secado;
                                equipo.Estado = _EstadosEquiposSecado.Semi_Ocupado;
                                EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].setPorcentajeMojado(100);
                                EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].simular(relojCentral);
                                equiposLibres = true;
                                break;
                            }
                        }

                        if (!equiposLibres)
                        {
                            foreach (EquipoSecado equipo in equiposDeSecado)
                            {
                                if (equipo.Estado == _EstadosEquiposSecado.Semi_Ocupado)
                                {
                                    equipo.Estado = _EstadosEquiposSecado.Ocupado_Total;
                                    if (equipo.Lugar1 == null)
                                    {
                                        equipo.Lugar1 = centro_B.getSiendoProcesado();
                                        equipo.Lugar1.setLugarSecado("Eq." + equipo.ID + "_1");
                                        equipo.Lugar1.Estado = _EstadosTrabajo.Siendo_Secado;
                                        EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].setPorcentajeMojado(100);
                                        EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].simular2Trabajos(relojCentral);
                                        EventosSecadoPosicionesSecundarias[(equipo.ID) - 1].actualizarSecadoA1Trabajo(relojCentral);
                                        EventosSecadoPosicionesSecundarias[(equipo.ID) - 1].simular2Trabajos(relojCentral);
                                    }
                                    else
                                    {
                                        equipo.Lugar2 = centro_B.getSiendoProcesado();
                                        equipo.Lugar2.setLugarSecado("Eq." + equipo.ID + "_2");
                                        equipo.Lugar2.Estado = _EstadosTrabajo.Siendo_Secado;
                                        EventosSecadoPosicionesSecundarias[(equipo.ID) - 1].setPorcentajeMojado(100);
                                        EventosSecadoPosicionesSecundarias[(equipo.ID) - 1].simular2Trabajos(relojCentral);
                                        EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].actualizarSecadoA1Trabajo(relojCentral);
                                        EventosSecadoPosicionesPrincipales[(equipo.ID) - 1].simular2Trabajos(relojCentral);
                                    }
                                    break;
                                }
                            }
                        }

                        siendoSecados.Add(centro_B.getSiendoProcesado());
                        centro_B.setSiendoProcesado(null);
                        fin_CentroB.setReinicio();
                        simuloCentroB = false;
                        fin_CentroB.setHoraFin(0.00);

                        bool equiposCompletos = true;
                        foreach (EquipoSecado equipo in equiposDeSecado)
                        {
                            if (equipo.Estado != _EstadosEquiposSecado.Ocupado_Total)
                            {
                                equiposCompletos = false;
                                break;
                            }
                        }

                        if (equiposCompletos)
                            centro_B.Estado = _EstadosCentros.Detenido;
                        else if (centro_B.tamañoCola() > 0)
                        {
                            Trabajo sacadoDeCola = centro_B.sacarDeCola();
                            sacadoDeCola.Estado = _EstadosTrabajo.Siendo_Procesado_CentroB;
                            centro_B.setSiendoProcesado(sacadoDeCola);
                            fin_CentroB.simular(relojCentral);
                            centro_B.Estado = _EstadosCentros.Ocupado;
                            simuloCentroB = true;

                            if (centro_A.Estado == _EstadosCentros.Detenido)
                            {
                                if (centro_A.tamañoCola() > 0)
                                {
                                    Trabajo sacadoCola = centro_A.sacarDeCola();
                                    sacadoCola.Estado = _EstadosTrabajo.Siendo_Procesado_CentroA;
                                    centro_A.setSiendoProcesado(sacadoCola);
                                    fin_CentroA.simular(relojCentral);
                                    centro_A.Estado = _EstadosCentros.Ocupado;
                                }
                                else
                                    centro_A.Estado = _EstadosCentros.Libre;

                                tiempoTotalParadaCentroA += (relojCentral - centro_A.InicioTiempoParada);
                                centro_A.InicioTiempoParada = 0.00;
                            }
                        }
                        else
                            centro_B.Estado = _EstadosCentros.Libre;

                        if (!bandera_AgregadosActivos)
                        {
                            if (cantidadEventosSimulados == limiteEventoMenor || (relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor && relojCentral != 0.00))
                            {
                                agregarTrabajosActivos(centro_A.getSiendoProcesado(), centro_A.getCola(), centro_B.getSiendoProcesado(), centro_B.getCola(), siendoSecados);
                                bandera_AgregadosActivos = true;
                            }
                        }

                        if ((relojCentral > limiteTiempoMenor && relojCentral < limiteTiempoMayor) ||
                            (cantidadEventosSimulados >= limiteEventoMenor && cantidadEventosSimulados < limiteEventoMayor))
                        {
                            i = dgv_Simulacion.Rows.Add();
                            agregarNuevaFila(siguiente, false, centro_A.getSiendoProcesado(), centro_B.getSiendoProcesado(), siendoSecados, i);
                            if (i != 1)
                                dgv_Simulacion.Rows[i - 1].Cells["finCentroB"].Style.BackColor = Color.Cyan;
                        }
                    }
                    else if (siguiente is FinSecado)
                    {
                        int i = 0;
                        FinSecado finSecado_Actual = (FinSecado)siguiente;
                        EquipoSecado equipoActual = finSecado_Actual.getEquipo();
                        Trabajo trabajoFinalizado;

                        if (finSecado_Actual.getNroOrden() == 1)
                        {
                            trabajoFinalizado = equipoActual.Lugar1;
                            equipoActual.Lugar1 = null;
                            EventosSecadoPosicionesPrincipales[(equipoActual.ID) - 1].setHoraFin(0.00);
                            EventosSecadoPosicionesPrincipales[(equipoActual.ID) - 1].setPorcentajeMojado(0.00);
                            EventosSecadoPosicionesPrincipales[(equipoActual.ID) - 1].setTiempoHastaFin(0.00);
                            if (equiposDeSecado[(equipoActual.ID) - 1].Estado == _EstadosEquiposSecado.Ocupado_Total)
                            {
                                EventosSecadoPosicionesSecundarias[(equipoActual.ID) - 1].actualizarSecadoA2Trabajos(relojCentral);
                                EventosSecadoPosicionesSecundarias[(equipoActual.ID) - 1].simular(relojCentral);
                                equiposDeSecado[(equipoActual.ID) - 1].Estado = _EstadosEquiposSecado.Semi_Ocupado;
                            }
                            else
                                equiposDeSecado[(equipoActual.ID) - 1].Estado = _EstadosEquiposSecado.Libre;
                        }
                        else
                        {
                            trabajoFinalizado = equipoActual.Lugar2;
                            equipoActual.Lugar2 = null;
                            EventosSecadoPosicionesSecundarias[(equipoActual.ID) - 1].setHoraFin(0.00);
                            EventosSecadoPosicionesSecundarias[(equipoActual.ID) - 1].setPorcentajeMojado(0.00);
                            EventosSecadoPosicionesSecundarias[(equipoActual.ID) - 1].setTiempoHastaFin(0.00);
                            if (equiposDeSecado[(equipoActual.ID) - 1].Estado == _EstadosEquiposSecado.Ocupado_Total)
                            {
                                EventosSecadoPosicionesPrincipales[(equipoActual.ID) - 1].actualizarSecadoA2Trabajos(relojCentral);
                                EventosSecadoPosicionesPrincipales[(equipoActual.ID) - 1].simular(relojCentral);
                                equiposDeSecado[(equipoActual.ID) - 1].Estado = _EstadosEquiposSecado.Semi_Ocupado;
                            }
                            else
                                equiposDeSecado[(equipoActual.ID) - 1].Estado = _EstadosEquiposSecado.Libre;
                        }

                        for (int j = 0; j < siendoSecados.Count; j++)
                        {
                            if (siendoSecados[j].Id == trabajoFinalizado.Id)
                                siendoSecados.RemoveAt(j);
                        }

                        acumuladoTrabajosTerminados += (relojCentral - trabajoFinalizado.getHoraInicioTrabajo());
                        cantidadTrabajosTerminados++;

                        if (centro_B.Estado == _EstadosCentros.Detenido)
                        {
                            if (centro_B.tamañoCola() > 0)
                            {
                                Trabajo sacadoDeCola = centro_B.sacarDeCola();
                                sacadoDeCola.Estado = _EstadosTrabajo.Siendo_Procesado_CentroB;
                                centro_B.setSiendoProcesado(sacadoDeCola);
                                fin_CentroB.simular(relojCentral);
                                centro_B.Estado = _EstadosCentros.Ocupado;

                                if (centro_A.Estado == _EstadosCentros.Detenido)
                                {
                                    if (centro_A.tamañoCola() > 0)
                                    {
                                        Trabajo sacadoCola = centro_A.sacarDeCola();
                                        sacadoCola.Estado = _EstadosTrabajo.Siendo_Procesado_CentroA;
                                        centro_A.setSiendoProcesado(sacadoCola);
                                        fin_CentroA.simular(relojCentral);
                                        centro_A.Estado = _EstadosCentros.Ocupado;
                                    }
                                    else
                                        centro_A.Estado = _EstadosCentros.Libre;

                                    tiempoTotalParadaCentroA += (relojCentral - centro_A.InicioTiempoParada);
                                    centro_A.InicioTiempoParada = 0.00;
                                }
                            }
                            else
                                centro_B.Estado = _EstadosCentros.Libre;
                        }

                        if (!bandera_AgregadosActivos)
                        {
                            if (cantidadEventosSimulados == limiteEventoMenor || (relojCentral >= limiteTiempoMenor && relojCentral < limiteTiempoMayor && relojCentral != 0.00))
                            {
                                agregarTrabajosActivos(centro_A.getSiendoProcesado(), centro_A.getCola(), centro_B.getSiendoProcesado(), centro_B.getCola(), siendoSecados);
                                bandera_AgregadosActivos = true;
                            }
                        }

                        if ((relojCentral > limiteTiempoMenor && relojCentral < limiteTiempoMayor) ||
                            (cantidadEventosSimulados >= limiteEventoMenor && cantidadEventosSimulados < limiteEventoMayor))
                        {
                            i = dgv_Simulacion.Rows.Add();
                            agregarNuevaFila(siguiente, false, centro_A.getSiendoProcesado(), centro_B.getSiendoProcesado(), siendoSecados, i);
                            if (i != 1)
                                dgv_Simulacion.Rows[i - 1].Cells["finSecado" + finSecado_Actual.getNroOrden() + "Equipo" + (equipoActual.ID)].Style.BackColor = Color.Cyan;
                        }
                    }

                    if (centro_A.getSiendoProcesado() != null)
                        trabajosActivosAhora++;
                    if (centro_B.getSiendoProcesado() != null)
                        trabajosActivosAhora++;
                    trabajosActivosAhora += centro_A.tamañoCola() + centro_B.tamañoCola() + siendoSecados.Count();

                    if (trabajosActivosAhora > cantMaxTrabajos)
                        cantMaxTrabajos = trabajosActivosAhora;

                    trabajosActivosAhora = 0;
                }
            }
            else
            {
                MessageBox.Show("Valores ingresados incorrectos. Parámetros vacíos o incorrectos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                invalido.Focus();
            }

            txtMaximoTrabajosActivos.Text = cantMaxTrabajos.ToString();

            if (tiempoTotalParadaCentroA > 0)
                txtTiempoParadoCentroA.Text = Math.Round(tiempoTotalParadaCentroA, 2).ToString();
            if (cantidadTrabajosTerminados > 0)
                txtTiempoPromedioTrabajo.Text = Math.Round((acumuladoTrabajosTerminados / cantidadTrabajosTerminados), 2).ToString();
            else
                txtTiempoPromedioTrabajo.Text = "0.00";

            this.Cursor = Cursors.Default;
        }

        // Agrega a la tabla las columnas correspondientes a todos los trabajos activos en el taller en ese momento.
        private void agregarTrabajosActivos(Trabajo enProcesoA, Queue<Trabajo> enColaA, Trabajo enProcesoB, Queue<Trabajo> enColaB, List<Trabajo> enSecado)
        {
            List<Trabajo> trabajosActivos = new List<Trabajo>();
            foreach (Trabajo trabajoEnCola in enColaA)
                trabajosActivos.Add(trabajoEnCola);
            if (enProcesoA != null)
                trabajosActivos.Add(enProcesoA);
            foreach (Trabajo trabajoEnCola in enColaB)
                trabajosActivos.Add(trabajoEnCola);
            if (enProcesoB != null)
                trabajosActivos.Add(enProcesoB);
            foreach (Trabajo trabajoSiendoSecado in enSecado)
                trabajosActivos.Add(trabajoSiendoSecado);

            trabajosActivos.Sort((p, q) => p.getHoraInicioTrabajo().CompareTo(q.getHoraInicioTrabajo()));

            foreach (Trabajo trabajoActivo in trabajosActivos)
            {
                dgv_Simulacion.Columns.Add(trabajoActivo.Id.ToString(), "Estado Trbj " + trabajoActivo.Id.Split('_')[1]);
                dgv_Simulacion.Columns.Add("horaInicioTrabajo" + trabajoActivo.Id.Split('_')[1], "Inicio Trbj " + trabajoActivo.Id.Split('_')[1]);
                dgv_Simulacion.Columns["horaInicioTrabajo" + trabajoActivo.Id.Split('_')[1]].Width = 50;
            }
        }

        private void agregarNuevaFila(Evento siguiente, bool conRandom, Trabajo procesadoCentroA, Trabajo procesadoCentroB, List<Trabajo> secados, int i)
        {
            agregarProximoTrabajoAGrilla(i, conRandom);
            agregarTrabajosAGrilla(centro_A.getSiendoProcesado(), centro_B.getSiendoProcesado(), siendoSecados, i);
            agregarDatosAGrilla(siguiente, i);
            agregarEquiposDeSecado(i);
        }

        // Actualiza los valores correspondientes a las columnas del evento LlegadaTrabajo.
        private void agregarProximoTrabajoAGrilla(int i, bool conRandom)
        {
            if (conRandom)
            {
                dgv_Simulacion.Rows[i].Cells["rndLlegada"].Value = llegada_Trabajo.getRandom();
                dgv_Simulacion.Rows[i].Cells["tiempoEntre"].Value = llegada_Trabajo.getTiempoEntreLlegada();
            }
            else
            {
                dgv_Simulacion.Rows[i].Cells["rndLlegada"].Value = "-";
                dgv_Simulacion.Rows[i].Cells["tiempoEntre"].Value = "-";
            }
            dgv_Simulacion.Rows[i].Cells["proxLlegada"].Value = llegada_Trabajo.getProximaLlegada();
        }

        // Actualiza los valores correspondientes a todos los trabajos que van pasando por el taller.
        private void agregarTrabajosAGrilla(Trabajo trabajoActualCentroA, Trabajo trabajoActualCentroB, List<Trabajo> trabajosSecandose, int i)
        {
            if (trabajoActualCentroA != null)
            {
                dgv_Simulacion.Rows[i].Cells[trabajoActualCentroA.Id].Value = trabajoActualCentroA.Estado;
                dgv_Simulacion.Rows[i].Cells[trabajoActualCentroA.Id.Replace("estadoTrabajo_", "horaInicioTrabajo")].Value = Math.Round(trabajoActualCentroA.getHoraInicioTrabajo(), 2);
            }

            if (trabajoActualCentroB != null)
            {
                dgv_Simulacion.Rows[i].Cells[trabajoActualCentroB.Id].Value = trabajoActualCentroB.Estado;
                dgv_Simulacion.Rows[i].Cells[trabajoActualCentroB.Id.Replace("estadoTrabajo_", "horaInicioTrabajo")].Value = Math.Round(trabajoActualCentroB.getHoraInicioTrabajo(), 2);
            }

            foreach (Trabajo trabajo in centro_A.getCola())
            {
                dgv_Simulacion.Rows[i].Cells[trabajo.Id].Value = trabajo.Estado;
                dgv_Simulacion.Rows[i].Cells[trabajo.Id.Replace("estadoTrabajo_", "horaInicioTrabajo")].Value = Math.Round(trabajo.getHoraInicioTrabajo(), 2);
            }

            foreach (Trabajo trabajo in centro_B.getCola())
            {
                dgv_Simulacion.Rows[i].Cells[trabajo.Id].Value = trabajo.Estado;
                dgv_Simulacion.Rows[i].Cells[trabajo.Id.Replace("estadoTrabajo_", "horaInicioTrabajo")].Value = Math.Round(trabajo.getHoraInicioTrabajo(), 2);
            }

            foreach (Trabajo trabajo in trabajosSecandose)
            {
                dgv_Simulacion.Rows[i].Cells[trabajo.Id].Value = trabajo.Estado + "_" + trabajo.getLugarSecado();
                dgv_Simulacion.Rows[i].Cells[trabajo.Id.Replace("estadoTrabajo_", "horaInicioTrabajo")].Value = Math.Round(trabajo.getHoraInicioTrabajo(), 2);
            }
        }

        // Actualiza los valores correspondientes a los eventos de FinSecado y los objetos EquipoSecado.
        private void agregarEquiposDeSecado(int i)
        {
            // -------------    Porcentaje Mojado   --------------
            for (int k = 0; k < EventosSecadoPosicionesPrincipales.Count(); k++)
            {
                if (EventosSecadoPosicionesPrincipales[k].getPorcentajeMojado() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID + "_1"].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID + "_1"].Value = Math.Round(EventosSecadoPosicionesPrincipales[k].getPorcentajeMojado(), 4);
            }

            for (int k = 0; k < EventosSecadoPosicionesSecundarias.Count(); k++)
            {
                if (EventosSecadoPosicionesSecundarias[k].getPorcentajeMojado() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID + "_2"].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["inicioPorcentaje_Eq" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID + "_2"].Value = Math.Round(EventosSecadoPosicionesSecundarias[k].getPorcentajeMojado(), 4);
            }

            // -------------    Tiempo De Secado   --------------
            for (int k = 0; k < EventosSecadoPosicionesPrincipales.Count(); k++)
            {
                if (EventosSecadoPosicionesPrincipales[k].getTiempoHastaFin() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado1_Eq" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID].Value = Math.Round(EventosSecadoPosicionesPrincipales[k].getTiempoHastaFin(), 2);
            }

            for (int k = 0; k < EventosSecadoPosicionesSecundarias.Count(); k++)
            {
                if (EventosSecadoPosicionesSecundarias[k].getTiempoHastaFin() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["tiempoCalculado2_Eq" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID].Value = Math.Round(EventosSecadoPosicionesSecundarias[k].getTiempoHastaFin(), 2);
            }

            // -------------    Hora Fin Secado --------------
            for (int k = 0; k < EventosSecadoPosicionesPrincipales.Count(); k++)
            {
                if (EventosSecadoPosicionesPrincipales[k].getProximaLlegada() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["finSecado1Equipo" + EventosSecadoPosicionesPrincipales[k].getEquipo().ID].Value = Math.Round(EventosSecadoPosicionesPrincipales[k].getProximaLlegada(), 2);
            }

            for (int k = 0; k < EventosSecadoPosicionesSecundarias.Count(); k++)
            {
                if (EventosSecadoPosicionesSecundarias[k].getProximaLlegada() == 0.00)
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID].Value = "-";
                else
                    dgv_Simulacion.Rows[i].Cells["finSecado2Equipo" + EventosSecadoPosicionesSecundarias[k].getEquipo().ID].Value = Math.Round(EventosSecadoPosicionesSecundarias[k].getProximaLlegada(), 2);
            }

            // -------------  Estado Equipos de Secado --------------
            for (int k = 0; k < equiposDeSecado.Count(); k++)
            {
                dgv_Simulacion.Rows[i].Cells["estado_EquipoDeSecado" + equiposDeSecado[k].ID].Value = equiposDeSecado[k].Estado.ToString();
            }
        }

        // Actualiza los valores correspondientes a: Los eventos FinCentroA, FinCentroB; los objetos Centro_A y Centro_B; y los estadísticos.
        private void agregarDatosAGrilla(Evento siguiente, int i)
        {
            if (String.Compare(siguiente.getNombreEvento(), "Fin_Secado") == 0)
            {
                FinSecado finSecado_Actual = (FinSecado)siguiente;
                dgv_Simulacion.Rows[i].Cells["evento"].Value = finSecado_Actual.getNombreEvento() + "_Eq." + finSecado_Actual.getEquipo().ID + "_" + finSecado_Actual.getNroOrden();
            }
            else
                dgv_Simulacion.Rows[i].Cells["evento"].Value = siguiente.getNombreEvento();
            dgv_Simulacion.Rows[i].Cells["relojGrilla"].Value = relojCentral;


            if (fin_CentroA.getProximaLlegada() == 0.00)
                dgv_Simulacion.Rows[i].Cells["finCentroA"].Value = "-";
            else
                dgv_Simulacion.Rows[i].Cells["finCentroA"].Value = Math.Round(fin_CentroA.getProximaLlegada(), 2);

            if (simuloCentroA)
            {
                dgv_Simulacion.Rows[i].Cells["rndCentroA"].Value = fin_CentroA.getRandomUsado();
                dgv_Simulacion.Rows[i].Cells["tiempoProcesoA"].Value = Math.Round(fin_CentroA.getTiempoProceso(), 4);
                simuloCentroA = false;
            }
            else
            {
                dgv_Simulacion.Rows[i].Cells["rndCentroA"].Value = "-";
                dgv_Simulacion.Rows[i].Cells["tiempoProcesoA"].Value = "-";
            }


            if (fin_CentroB.getProximaLlegada() == 0.00)
                dgv_Simulacion.Rows[i].Cells["finCentroB"].Value = "-";
            else
                dgv_Simulacion.Rows[i].Cells["finCentroB"].Value = Math.Round(fin_CentroB.getProximaLlegada(), 2);

            if (simuloCentroB)
            {
                dgv_Simulacion.Rows[i].Cells["rnd1CentroB"].Value = fin_CentroB.getRandomUsados()[0];
                dgv_Simulacion.Rows[i].Cells["rnd2CentroB"].Value = fin_CentroB.getRandomUsados()[1];
                dgv_Simulacion.Rows[i].Cells["tiempoProcesoB"].Value = Math.Round(fin_CentroB.getTiempoProceso(), 4);
                simuloCentroB = false;
            }
            else
            {
                dgv_Simulacion.Rows[i].Cells["rnd1CentroB"].Value = "-";
                dgv_Simulacion.Rows[i].Cells["rnd2CentroB"].Value = "-";
                dgv_Simulacion.Rows[i].Cells["tiempoProcesoB"].Value = "-";
            }

            dgv_Simulacion.Rows[i].Cells["estadoCentroA"].Value = centro_A.Estado;
            dgv_Simulacion.Rows[i].Cells["colaCentroA"].Value = centro_A.tamañoCola();
            if (centro_A.Estado == _EstadosCentros.Detenido)
                dgv_Simulacion.Rows[i].Cells["inicioTiempoParada"].Value = Math.Round(centro_A.InicioTiempoParada, 2);
            else
                dgv_Simulacion.Rows[i].Cells["inicioTiempoParada"].Value = "-";
            dgv_Simulacion.Rows[i].Cells["estadoCentroB"].Value = centro_B.Estado;
            dgv_Simulacion.Rows[i].Cells["colaCentroB"].Value = centro_B.tamañoCola();
            dgv_Simulacion.Rows[i].Cells["maxTrabajosActivos"].Value = cantMaxTrabajos;
            dgv_Simulacion.Rows[i].Cells["tiempoParadaA"].Value = Math.Round(tiempoTotalParadaCentroA, 2);
            dgv_Simulacion.Rows[i].Cells["trabajosTerminados"].Value = cantidadTrabajosTerminados;
            dgv_Simulacion.Rows[i].Cells["tiempoTrabajoAcumulado"].Value = Math.Round(acumuladoTrabajosTerminados, 2);
        }
    }
}

