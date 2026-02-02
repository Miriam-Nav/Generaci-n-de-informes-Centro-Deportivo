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
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly ReservasService _reservaService = new ReservasService();
        private readonly ReservaRepositorio _reservaRepo = new ReservaRepositorio();
        private readonly SocioRepositorio _socioRepo = new SocioRepositorio();
        private readonly ActividadRepositorio _actividadRepo = new ActividadRepositorio();

        // LISTA DE RESERVAS
        private List<Reserva> _reservas;
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
        public List<Socio> Socios { get; set; }
        public List<Actividad> Actividades { get; set; }

        // RESERVA SELECCIONADA
        private Reserva _reservaSeleccionada;
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
        public ICommand CrearCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand GenerarInformeCommand { get; }
        public ICommand GenerarHistorialCommand { get; }

        // CONSTRUCTOR
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

        private void LimpiarFormulario()
        {
            InputSocioId = 0; 
            InputActividadId = 0; 
            InputFecha = DateTime.Now;
            LimpiarErrores();
            InputReservaSeleccionada = null;
        }

        private void LimpiarErrores()
        {
            ErrorSocio = "";
            ErrorActividad = "";
            ErrorFecha = "";
            ErrorAforo = "";
            ErrorSolapamiento = "";
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}