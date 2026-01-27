using Centro_Deportivo;
using Model;
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
        private CentroDeportivoEntities _db = new CentroDeportivoEntities();

        // LISTA DE RESERVAS
        private List<Reserva> _reservas;
        public List<Reserva> Reservas
        {
            get => _reservas;
            set
            {
                _reservas = value;
                OnPropertyChanged(nameof(Reservas));
            }
        }

        // LISTAS PARA COMBOS
        public List<Socio> Socios { get; set; }
        public List<Actividad> Actividades { get; set; }

        // RESERVA SELECCIONADA
        private Reserva _reservaSeleccionada;
        public Reserva ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged(nameof(ReservaSeleccionada));

                if (value != null)
                {
                    SocioId = value.SocioId;
                    ActividadId = value.ActividadId;
                    Fecha = value.Fecha;
                }
            }
        }

        // CAMPOS DEL FORMULARIO

        // SocioId
        private int _socioId;
        public int SocioId
        {
            get => _socioId;
            set
            {
                _socioId = value;
                OnPropertyChanged(nameof(SocioId));
            }
        }

        // ActividadId
        private int _actividadId;
        public int ActividadId
        {
            get => _actividadId;
            set
            {
                _actividadId = value;
                OnPropertyChanged(nameof(ActividadId));
            }
        }

        // Fecha
        private DateTime _fecha = DateTime.Now;
        public DateTime Fecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged(nameof(Fecha));
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

            try
            {
                List<Reserva> listaReservas = _db.Reserva.ToList();

                // Filtra por los socios activos.
                var sociosActivos = _db.Socio.Where(socio => socio.Activo).ToList();
                List<Socio> listaSocios = sociosActivos;

                List<Actividad> listaActividades = _db.Actividad.ToList();

                Reservas = listaReservas;
                Socios = listaSocios;
                Actividades = listaActividades;
            }
            catch (Exception ex) { 
                ErrorAforo = $"Error al cargar los datos: {ex.Message}"; 
            }

            CrearCommand = new RelayCommand(CrearReserva);
            ModificarCommand = new RelayCommand(ModificarReserva);
            EliminarCommand = new RelayCommand(EliminarReserva);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarInformeCommand = new RelayCommand(GenerarInformeActividad);
            GenerarHistorialCommand = new RelayCommand(GenerarInformeHistorial);
        }

        // VALIDACIONES
        // Validar Socio
        private bool ValidarSocio()
        {
            if (SocioId <= 0)
            {
                ErrorSocio = "Debes seleccionar un socio.";
                return false;
            }

            var socio = _db.Socio.Find(SocioId);
            if (socio == null)
            {
                ErrorSocio = "El socio seleccionado no existe.";
                return false;
            }

            if (!socio.Activo)
            {
                ErrorSocio = "El socio no está activo.";
                return false;
            }

            ErrorSocio = "";
            return true;
        }

        // Validar Actividad
        private bool ValidarActividad()
        {
            if (ActividadId <= 0)
            {
                ErrorActividad = "Debes seleccionar una actividad.";
                return false;
            }

            var actividad = _db.Actividad.Find(ActividadId);
            if (actividad == null)
            {
                ErrorActividad = "La actividad seleccionada no existe.";
                return false;
            }

            ErrorActividad = "";
            return true;
        }

        // Validar Fecha
        private bool ValidarFecha()
        {
            if (Fecha == default(DateTime))
            {
                ErrorFecha = "Debes seleccionar una fecha.";
                return false;
            }

            if (Fecha < DateTime.Now)
            {
                ErrorFecha = "No puedes reservar en una fecha pasada.";
                return false;
            }

            ErrorFecha = "";
            return true;
        }

        // Validar Aforo
        private bool ValidarAforo()
        {
            var actividad = _db.Actividad.Find(ActividadId);

            if (actividad == null)
            {
                ErrorAforo = "La actividad seleccionada no existe.";
                return false;
            }

            

            DateTime fecha = Fecha.Date;
            DateTime siguienteDia = fecha.AddDays(1);

            // Saca la cantidad de personas que han reservado esa actividad
            int idReserva = ReservaSeleccionada?.Id ?? 0;

            var reservas = _db.Reserva.ToList();
            int reservasActuales = reservas.Where(reserva =>
                reserva.ActividadId == ActividadId &&
                reserva.Id != idReserva &&
                reserva.Fecha.Date == fecha)
            .Count();



            if (reservasActuales >= actividad.AforoMaximo)
            {
                ErrorAforo = "La actividad está completa.";
                return false;
            }

            ErrorAforo = "";
            return true;
        }

        // Validar Solapamiento
        private bool ValidarSolapamiento(bool crear)
        {
            // Comrpueba si se esta creando o modificando
            int idReserva = 0;
            if (!crear)
            {
                idReserva = ReservaSeleccionada?.Id ?? 0;
            }

            // Comprueba si el socio ya ha reservado esa actividad ese día
            var reservas = _db.Reserva.ToList();

            bool solapa = reservas.Any(reserva =>
                reserva.SocioId == SocioId &&
                reserva.Id != idReserva &&
                reserva.Fecha.Date >= Fecha.Date
            );

            if (solapa)
            {
                ErrorSolapamiento = "Ya tiene una reserva de esa actividad en ese día.";
                return false;
            }

            ErrorSolapamiento = "";
            return true;
        }

        // Validar Formulario
        private bool ValidarFormulario(bool crear)
        {
            bool okSocio = ValidarSocio();
            bool okActividad = ValidarActividad();
            bool okFecha = ValidarFecha();
            bool okAforo = ValidarAforo();
            bool okSolapamiento = ValidarSolapamiento(crear);

            return okSocio && okActividad && okFecha && okAforo && okSolapamiento;
        }


        // CREAR RESERVA
        private void CrearReserva()
        {
            if (!ValidarFormulario(true))
            {
                return;
            }
            try
            {
                var nueva = new Reserva
                {
                    SocioId = SocioId,
                    ActividadId = ActividadId,
                    Fecha = Fecha
                };

                _db.Reserva.Add(nueva);
                _db.SaveChanges();

                Reservas = _db.Reserva.ToList();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorAforo = $"Error al guardar la reserva: {ex.Message}";
            }
        }

        // MODIFICAR RESERVA
        private void ModificarReserva()
        {
            if (ReservaSeleccionada == null) { 
                return; 
            }
            
            if (!ValidarFormulario(false)) { 
                return; 
            }

            try
            {
                ReservaSeleccionada.SocioId = SocioId;
                ReservaSeleccionada.ActividadId = ActividadId;
                ReservaSeleccionada.Fecha = Fecha;

                _db.SaveChanges();

                Reservas = _db.Reserva.ToList();
            }
            catch (Exception ex)
            {
                ErrorAforo = $"Error al modificar la reserva: {ex.Message}";
            }
        }

        // ELIMINAR RESERVA
        private void EliminarReserva()
        {
            if (ReservaSeleccionada == null) { 
                return; 
            }

            try
            {
                _db.Reserva.Remove(ReservaSeleccionada);
                _db.SaveChanges();

                Reservas = _db.Reserva.ToList();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorAforo = $"Error al eliminar la reserva: {ex.Message}";
            }
        }

        // LIMPIAR FORMULARIO
        private void LimpiarFormulario()
        {
            SocioId = 0;
            ActividadId = 0;
            Fecha = DateTime.Now;

            ErrorSocio = "";
            ErrorActividad = "";
            ErrorFecha = "";
            ErrorAforo = "";
            ErrorSolapamiento = "";

            ReservaSeleccionada = null;
        }

        private void GenerarInformeActividad()
        {
            try
            {
                if (this.ActividadId == 0)
                {
                    System.Windows.MessageBox.Show("Por favor, selecciona una actividad primero.");
                    return;
                }

                var miReporte = new crReservasActividad();

                var datos = _db.Reserva
                    .Where(r => r.ActividadId == this.ActividadId)
                    .Select(r => new
                    {
                        NombreActividad = r.Actividad.Nombre,
                        FechaReserva = r.Fecha,
                        NombreSocio = r.Socio.Nombre,
                        AforoMaximo = r.Actividad.AforoMaximo,
                        IdActividad = r.ActividadId
                    }).ToList();

                miReporte.SetDataSource(datos);

                miReporte.SetParameterValue("paramIdActividad", this.ActividadId);

                var ventanaVisor = new InformesView();
                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;
                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al generar el informe de actividad: {ex.Message}");
            }
        }

        private void GenerarInformeHistorial()
        {
            try
            {
                var miReporte = new crReservasSocio();

                var datos = _db.Reserva
                    .Select(r => new
                    {
                        NombreSocio = r.Socio.Nombre,
                        NombreActividad = r.Actividad.Nombre,
                        FechaReserva = r.Fecha
                    })
                    .OrderBy(r => r.FechaReserva) 
                    .ToList();

                miReporte.SetDataSource(datos);

                var ventanaVisor = new InformesView();
                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;
                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
           PropertyChanged?.Invoke(this, new
           PropertyChangedEventArgs(propertyName));
        }

    }
}
