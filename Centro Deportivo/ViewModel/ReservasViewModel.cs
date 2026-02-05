using Centro_Deportivo;
using Model;
using Model.Repositorios;
using ViewModel.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// ViewModel encargado de la gestión de reservas del centro deportivo.
    /// Coordina la relación entre socios y actividades, valida solapamientos y gestiona la generación de informes.
    /// </summary>
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly ReservasService _reservaService = new ReservasService();
        private readonly ReservaRepositorio _reservaRepo = new ReservaRepositorio();
        private readonly SocioRepositorio _socioRepo = new SocioRepositorio();
        private readonly ActividadRepositorio _actividadRepo = new ActividadRepositorio();

        // LISTA DE RESERVAS
        private List<Reserva> _reservas;
        /// <summary> Lista completa de reservas registradas en el sistema. </summary>
        public List<Reserva> InputReservas
        {
            get => _reservas;
            set
            {
                _reservas = value;
                OnPropertyChanged(nameof(InputReservas));
            }
        }

        // LISTAS PARA COMBOS
        /// <summary> Listado de socios activos disponibles para realizar reservas. </summary>
        public List<Socio> Socios { get; set; }
        /// <summary> Listado de todas las actividades deportivas ofertadas. </summary>
        public List<Actividad> Actividades { get; set; }

        // RESERVA SELECCIONADA
        private Reserva _reservaSeleccionada;
        /// <summary> Reserva seleccionada actualmente en el listado. Actualiza los campos del formulario al cambiar. </summary>
        public Reserva InputReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged(nameof(InputReservaSeleccionada));

                if (value != null)
                {
                    InputSocioId = value.SocioId;
                    InputActividadId = value.ActividadId;
                    InputFecha = value.Fecha;
                }
            }
        }

        // CAMPOS DEL FORMULARIO
        // SocioId
        private int _socioId;
        /// <summary> Identificador del socio seleccionado en el formulario. </summary>
        public int InputSocioId
        {
            get => _socioId;
            set
            {
                _socioId = value;
                OnPropertyChanged(nameof(InputSocioId));
            }
        }

        // ActividadId
        private int _actividadId;
        /// <summary> Identificador de la actividad seleccionada en el formulario. </summary>
        public int InputActividadId
        {
            get => _actividadId;
            set
            {
                _actividadId = value;
                OnPropertyChanged(nameof(InputActividadId));
            }
        }

        // Fecha
        private DateTime _fecha = DateTime.Now;
        /// <summary> Fecha y hora programada para la reserva introducida en el formulario. </summary>
        public DateTime InputFecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged(nameof(InputFecha));
            }
        }

        // MENSAJES DE ERROR
        // Error Socio
        private string _errorSocio;
        /// <summary> Almacena el mensaje de error relacionado a la selección o estado del socio. </summary>
        public string ErrorSocio
        {
            get => _errorSocio;
            set
            {
                _errorSocio = value;
                OnPropertyChanged(nameof(ErrorSocio));
            }
        }

        // Error Actividad
        private string _errorActividad;
        /// <summary> Almacena el mensaje de error relacionado a la actividad o su disponibilidad. </summary>
        public string ErrorActividad
        {
            get => _errorActividad;
            set
            {
                _errorActividad = value;
                OnPropertyChanged(nameof(ErrorActividad));
            }
        }

        // Error Fecha
        private string _errorFecha;
        /// <summary> Almacena el mensaje de error relacionado a la validación de la fecha. </summary>
        public string ErrorFecha
        {
            get => _errorFecha;
            set
            {
                _errorFecha = value;
                OnPropertyChanged(nameof(ErrorFecha));
            }
        }

        // Error Aforo
        private string _errorAforo;
        /// <summary> Almacena el mensaje de error relacionado a la falta de aforo en la actividad. </summary>
        public string ErrorAforo
        {
            get => _errorAforo;
            set
            {
                _errorAforo = value;
                OnPropertyChanged(nameof(ErrorAforo));
            }
        }

        // Error Solapamiento
        private string _errorSolapamiento;
        /// <summary> Almacena el mensaje de error cuando un socio intenta duplicar actividades en la misma hora. </summary>
        public string ErrorSolapamiento
        {
            get => _errorSolapamiento;
            set
            {
                _errorSolapamiento = value;
                OnPropertyChanged(nameof(ErrorSolapamiento));
            }
        }

        // COMMANDS
        /// <summary> Comando para registrar el alta de una nueva reserva. </summary>
        public ICommand CrearCommand { get; }

        /// <summary> Comando para modificar los datos de la reserva seleccionada. </summary>
        public ICommand ModificarCommand { get; }

        /// <summary> Comando para cancelar o eliminar la reserva seleccionada. </summary>
        public ICommand EliminarCommand { get; }

        /// <summary> Comando para resetear los campos del formulario y limpiar los mensajes de error. </summary>
        public ICommand LimpiarCommand { get; }

        /// <summary> Comando para generar el informe de una actividad específica. </summary>
        public ICommand GenerarInformeCommand { get; }

        /// <summary> Comando para generar el historial completo de reservas de un socio. </summary>
        public ICommand GenerarHistorialCommand { get; }

        // CONSTRUCTOR
        /// <summary>
        /// Constructor del ViewModel de Reservas.
        /// Carga los datos iniciales y vincula los comandos a sus métodos correspondientes.
        /// </summary>
        public ReservasViewModel()
        {
            RefrescarDatos();

            CrearCommand = new RelayCommand(CrearReserva);
            ModificarCommand = new RelayCommand(ModificarReserva);
            EliminarCommand = new RelayCommand(EliminarReserva);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarInformeCommand = new RelayCommand(GenerarInformeActividad);
            GenerarHistorialCommand = new RelayCommand(GenerarInformeHistorial);
        }

        /// <summary>
        /// Actualiza los listados de reservas, socios y actividades consultando a los repositorios.
        /// </summary>
        private void RefrescarDatos()
        {
            try
            {
                InputReservas = _reservaRepo.Seleccionar();
                Socios = _socioRepo.Seleccionar().Where(s => s.Activo).ToList();
                Actividades = _actividadRepo.Seleccionar();
            }
            catch (Exception ex) { 
                ErrorAforo = $"Error al cargar datos: {ex}"; 
            }
        }

        /// <summary>
        /// Crea una nueva reserva y gestiona posibles excepciones.
        /// </summary>
        private void CrearReserva()
        {

            try
            {
                LimpiarErrores();

                var nueva = new Reserva { SocioId = InputSocioId, ActividadId = InputActividadId, Fecha = InputFecha };
                var actividad = Actividades.FirstOrDefault(a => a.Id == InputActividadId);

                _reservaService.CrearReserva(nueva, actividad);

                RefrescarDatos();
                LimpiarFormulario();
            }
            catch (Exception ex) {
                Errores(ex);
            }
        }

        /// <summary>
        /// Modifica los datos de la reserva seleccionada y gestiona posibles excepciones.
        /// </summary>
        private void ModificarReserva()
        {
            if (InputReservaSeleccionada == null)
            {
                return;
            }

            try
            {
                LimpiarErrores();

                InputReservaSeleccionada.SocioId = InputSocioId;
                InputReservaSeleccionada.ActividadId = InputActividadId;
                InputReservaSeleccionada.Fecha = InputFecha;

                _reservaService.ActualizarReserva(InputReservaSeleccionada);
                RefrescarDatos();
            }
            catch (Exception ex) {
                Errores(ex);
            }
        }

        /// <summary>
        /// Gestiona la eliminación de la reserva seleccionada, capturando errores.
        /// </summary>
        private void EliminarReserva()
        {
            LimpiarErrores();

            if (InputReservaSeleccionada == null)
            {
                return;
            }
            _reservaService.EliminarReserva(InputReservaSeleccionada);
            RefrescarDatos();
            LimpiarFormulario();
        }

        /// <summary>
        /// Método que distribuye los mensajes de error de las excepciones hacia la interfaz.
        /// </summary>
        private void Errores(Exception ex)
        {
            string msg = ex.Message;

            if (msg.Contains("socio"))
            {
                ErrorSocio = msg;
            }
            else if (msg.Contains("actividad") || msg.Contains("aforo"))
            {
                ErrorActividad = msg;
            }
            else if (msg.Contains("fecha"))
            {
                ErrorFecha = msg;
            }
            else
            {
                ErrorSolapamiento = msg;
            }

        }

        /// <summary>
        /// Reinicia las propiedades del formulario de reservas a su estado inicial.
        /// </summary>
        private void LimpiarFormulario()
        {
            InputSocioId = 0; 
            InputActividadId = 0; 
            InputFecha = DateTime.Now;
            LimpiarErrores();
            InputReservaSeleccionada = null;
        }
        
        /// <summary>
        /// Limpia todos los mensajes de error activos en la interfaz de reservas.
        /// </summary>
        private void LimpiarErrores()
        {
            ErrorSocio = "";
            ErrorActividad = "";
            ErrorFecha = "";
            ErrorAforo = "";
            ErrorSolapamiento = "";
        }

        /// <summary>
        /// Genera y muestra un informe con los inscritos en la actividad seleccionada.
        /// </summary>
        private void GenerarInformeActividad()
        {
            try
            {
                if (this.InputActividadId == 0)
                {
                    ErrorActividad = "Selecciona una actividad para generar el informe.";
                    return;
                }

                var miReporte = new crReservasActividad();

                using (var db = new CentroDeportivoEntities())
                {
                    var datos = db.Reserva
                        .Where(r => r.ActividadId == this.InputActividadId)
                        .Select(r => new
                        {
                            NombreActividad = r.Actividad.Nombre,
                            FechaReserva = r.Fecha,
                            NombreSocio = r.Socio.Nombre,
                            AforoMaximo = r.Actividad.AforoMaximo,
                            IdActividad = r.ActividadId
                        }).ToList();

                    miReporte.SetDataSource(datos);
                }

                miReporte.SetParameterValue("paramIdActividad", this.InputActividadId);

                var ventanaVisor = new InformesView();
                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;
                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorAforo = $"Error en informe: {ex.Message}";
            }
        }

        /// <summary>
        /// Genera y muestra un informe con el historial de todas las reservas registradas.
        /// </summary>
        private void GenerarInformeHistorial()
        {
            try
            {
                var miReporte = new crReservasSocio();

                using (var db = new CentroDeportivoEntities())
                {
                    var datos = db.Reserva
                        .Select(r => new
                        {
                            NombreSocio = r.Socio.Nombre,
                            NombreActividad = r.Actividad.Nombre,
                            FechaReserva = r.Fecha
                        })
                        .OrderBy(r => r.FechaReserva)
                        .ToList();

                    miReporte.SetDataSource(datos);
                }

                var ventanaVisor = new InformesView();
                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;
                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorAforo = $"Error en historial: {ex.Message}";
            }
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> Invoca el evento PropertyChanged para una propiedad específica. </summary>
        /// <param name="propertyName">Nombre de la propiedad modificada.</param>
        protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}