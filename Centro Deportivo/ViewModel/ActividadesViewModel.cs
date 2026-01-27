using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ViewModel
{
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        private CentroDeportivoEntities _db = new CentroDeportivoEntities();

        // LISTA DE ACTIVIDADES
        private List<Actividad> _actividades;
        public List<Actividad> Actividades
        {
            // Devuelve la colección privada _actividades
            get => _actividades;
            set
            {
                // Guarda el nuevo valor y notifica el cambio
                _actividades = value;
                OnPropertyChanged(nameof(Actividades));
            }
        }

        // ACTIVIDAD SELECCIONADA
        private Actividad _actividadSeleccionada;
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));

                if (value != null)
                {
                    NombreActividad = value.Nombre;
                    AforoMaximo = value.AforoMaximo.ToString();
                }
            }
        }

        // CAMPOS DEL FORMULARIO

        // Nombre Actividad
        private string _nombreActividad;
        public string NombreActividad
        {
            get => _nombreActividad;
            set
            {
                _nombreActividad = value;
                OnPropertyChanged(nameof(NombreActividad));
            }
        }

        // Aforo Maximo
        private string _aforoMaximo;
        public string AforoMaximo
        {
            get => _aforoMaximo;
            set
            {
                _aforoMaximo = value;
                OnPropertyChanged(nameof(AforoMaximo));
            }
        }

        // MENSAJES DE ERROR
        // Error Nombre
        private string _errorNombre;
        public string ErrorNombre
        {
            get => _errorNombre;
            set
            {
                _errorNombre = value;
                OnPropertyChanged(nameof(ErrorNombre));
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

        // COMMANDS
        public ICommand CrearCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }

        // CONSTRUCTOR
        public ActividadesViewModel()
        {
            try
            {
                var actividades = _db.Actividad.ToList();
                Actividades = new List<Actividad>(actividades);
            }
            catch (Exception ex)
            {
                ErrorActividad = $"Error al cargar los datos: {ex.Message}";
            }

            CrearCommand = new RelayCommand(CrearActividad);
            ModificarCommand = new RelayCommand(ModificarActividad);
            EliminarCommand = new RelayCommand(EliminarActividad);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
        }

        // VALIDACIONES
        // Validación Nombre
        private bool ValidarNombreCrear()
        {
            if (string.IsNullOrWhiteSpace(NombreActividad))
            {
                ErrorNombre = "El nombre no puede estar vacío";
                return false;
            }

            bool nombreExiste = _db.Actividad.Any(actividad => actividad.Nombre == NombreActividad);

            if (nombreExiste)
            {
                ErrorNombre = "Ya existe una actividad con este nombre";
                return false;
            }

            ErrorNombre = "";
            return true;
        }

        private bool ValidarNombreModificar()
        {
            if (string.IsNullOrWhiteSpace(NombreActividad))
            {
                ErrorNombre = "El nombre no puede estar vacío";
                return false;
            }

            bool nombreExiste = _db.Actividad.Any(actividad => actividad.Nombre == NombreActividad && actividad.Id != ActividadSeleccionada.Id);

            if (nombreExiste)
            {
                ErrorNombre = "Ya existe una actividad con este nombre";
                return false;
            }

            ErrorNombre = "";
            return true;
        }

        // Validación Aforo
        private bool ValidarAforo()
        {
            if (string.IsNullOrWhiteSpace(AforoMaximo))
            {
                ErrorAforo = "Debes introducir un número";
                return false;
            }

            if (!int.TryParse(AforoMaximo, out int aforo))
            {
                ErrorAforo = "Debes introducir un número";
                return false;
            }

            if (int.Parse(AforoMaximo) <= 0)
            {
                ErrorAforo = "El aforo debe ser mayor que 0";
                return false;
            }

            ErrorAforo = "";
            return true;
        }

        // Validación Formulario
        private bool ValidarFormulario(bool crear)
        {
            bool okNombre = false;
            if (crear)
            {
                okNombre = ValidarNombreCrear();
            }
            else
            {
                okNombre = ValidarNombreModificar();
            }

            bool okAforo = ValidarAforo();

            return okNombre && okAforo;
        }

        // CREAR ACTIVIDAD
        private void CrearActividad()
        {
            // Comprueba que todas las validaciones del formulario sean true
            bool resultado = ValidarFormulario(true);

            if (!resultado)
            {
                return;
            }

            try
            {
                var nueva = new Actividad
                {
                    Nombre = NombreActividad,
                    AforoMaximo = int.Parse(AforoMaximo)
                };

                _db.Actividad.Add(nueva);
                _db.SaveChanges();

                Actividades = _db.Actividad.ToList();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorActividad = $"Error al crear la actividad: {ex.Message}";
            }
        }

        // MODIFICAR ACTIVIDAD
        private void ModificarActividad()
        {
            if (ActividadSeleccionada == null)
            {
                return;
            }

            bool resultado = ValidarFormulario(false);

            if (!resultado)
            {
                return;
            }
            try
            {

                ActividadSeleccionada.Nombre = NombreActividad;
                ActividadSeleccionada.AforoMaximo = int.Parse(AforoMaximo);

                _db.SaveChanges();

                Actividades = _db.Actividad.ToList();
            }
            catch (Exception ex)
            {
                ErrorActividad = $"Error al modificar la actividad: {ex.Message}";
            }
        }

        // ELIMINAR ACTIVIDAD
        private void EliminarActividad()
        {
            if (ActividadSeleccionada == null)
            {
                return;
            }
            try
            {

                // Validar si tiene reservas
                bool tieneReservas = _db.Reserva.Any(reserva => reserva.ActividadId == ActividadSeleccionada.Id);

                if (tieneReservas)
                {
                    ErrorActividad = "No se puede eliminar: la actividad tiene reservas asignadas.";
                    return;
                }

                _db.Actividad.Remove(ActividadSeleccionada);
                _db.SaveChanges();

                Actividades = _db.Actividad.ToList();
                LimpiarFormulario();

            }
            catch (Exception ex)
            {
                ErrorActividad = $"Error al eliminar la actividad: {ex.Message}";
            }
        }

        // LIMPIAR FORMULARIO
        private void LimpiarFormulario()
        {
            NombreActividad = "";
            AforoMaximo = null;
            ErrorNombre = "";
            ErrorAforo = "";
            ErrorActividad = "";
            ActividadSeleccionada = null;
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
