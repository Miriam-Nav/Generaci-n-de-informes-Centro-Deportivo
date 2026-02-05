using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel encargado de la gestión de actividades deportivas.
    /// Controla la lógica de creación, modificación y borrado, así como la gestión de errores de aforo.
    /// </summary>
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        private readonly ActividadesService _actividadService = new ActividadesService();
        private readonly ActividadRepositorio _repo = new ActividadRepositorio();

        // LISTA DE ACTIVIDADES
        private List<Actividad> _actividades;
        /// <summary> Lista completa de actividades registradas en el sistema. </summary>
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
        /// <summary> Actividad seleccionada actualmente en el listado. Actualiza los campos del formulario al cambiar. </summary>
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));

                if (value != null)
                {
                    InputNombreActividad = value.Nombre;
                    InputAforoMaximo = value.AforoMaximo.ToString();
                }
            }
        }

        // CAMPOS DEL FORMULARIO

        // Nombre Actividad
        private string _nombreActividad;
        /// <summary> Nombre de la actividad introducido en el formulario. </summary>
        public string InputNombreActividad
        {
            get => _nombreActividad;
            set
            {
                _nombreActividad = value;
                OnPropertyChanged(nameof(InputNombreActividad));
            }
        }

        // Aforo Maximo
        private string _aforoMaximo;
        /// <summary> Valor del aforo máximo introducido en el formulario. </summary>
        public string InputAforoMaximo
        {
            get => _aforoMaximo;
            set
            {
                _aforoMaximo = value;
                OnPropertyChanged(nameof(InputAforoMaximo));
            }
        }

        // MENSAJES DE ERROR
        // Error Nombre
        private string _errorNombre;
        /// <summary> Almacena el mensaje de error relacionado al nombre de la actividad. </summary>
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
        /// <summary> Almacena el mensaje de error general de la sección de actividades. </summary>
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
        /// <summary> Almacena el mensaje de error relacionado a la validación del aforo. </summary>
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
        /// <summary> Comando para registrar una nueva actividad. </summary>
        public ICommand CrearCommand { get; }

        /// <summary> Comando para actualizar los datos de la actividad seleccionada. </summary>
        public ICommand ModificarCommand { get; }

        /// <summary> Comando para eliminar la actividad seleccionada del sistema. </summary>
        public ICommand EliminarCommand { get; }

        /// <summary> Comando para resetear los campos del formulario y limpiar los mensajes de error. </summary>
        public ICommand LimpiarCommand { get; }

        // CONSTRUCTOR
        /// <summary>
        /// Constructor del ViewModel de Actividades.
        /// Carga la lista inicial y vincula los comandos a sus métodos.
        /// </summary>
        public ActividadesViewModel()
        {
            RefrescarLista();
            CrearCommand = new RelayCommand(CrearActividad);
            ModificarCommand = new RelayCommand(ModificarActividad);
            EliminarCommand = new RelayCommand(EliminarActividad);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
        }

        /// <summary>
        /// Actualiza la colección de actividades consultando al repositorio.
        /// </summary>
        private void RefrescarLista()
        {
            Actividades = _repo.Seleccionar();
        }

        /// <summary>
        /// Limpia todos los mensajes de error activos en la interfaz.
        /// </summary>
        private void LimpiarErrores() {

                ErrorNombre = "";

                ErrorAforo = "";
           
                ErrorActividad = "";
        }

        /// <summary>
        /// Crea una nueva actividad y gestiona posibles excepciones.
        /// </summary>
        private void CrearActividad()
        {
            try
            {
                LimpiarErrores();

                int aforo = 0;

                if (int.TryParse(InputAforoMaximo, out int result))
                {
                    aforo = result;
                }

                var nueva = new Actividad { Nombre = InputNombreActividad, AforoMaximo = aforo };

                _actividadService.CrearActividad(nueva);

                RefrescarLista();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                // REPARTIDOR DE ERRORES
                if (ex.Message.Contains("nombre"))
                    ErrorNombre = ex.Message;
                else if (ex.Message.Contains("aforo"))
                    ErrorAforo = ex.Message;
                else
                    ErrorActividad = ex.Message;
            }
        }

        /// <summary>
        /// Modifica los datos de la actividad seleccionada, la actualiza y gestiona posibles excepciones.
        /// </summary>
        private void ModificarActividad()
        {
            if (ActividadSeleccionada == null) return;

            try
            {
                LimpiarErrores();

                int aforo = 0;

                if (int.TryParse(InputAforoMaximo, out int result))
                {
                    aforo = result;
                }

                ActividadSeleccionada.Nombre = InputNombreActividad;
                ActividadSeleccionada.AforoMaximo = aforo;

                _actividadService.ActualizarActividad(ActividadSeleccionada);

                RefrescarLista();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                // REPARTIDOR DE ERRORES
                if (ex.Message.Contains("nombre"))
                    ErrorNombre = ex.Message;
                else if (ex.Message.Contains("aforo"))
                    ErrorAforo = ex.Message;
                else
                    ErrorActividad = ex.Message;
            }
        }

        /// <summary>
        /// Gestiona la eliminación de una actividad, capturando errores.
        /// </summary>
        private void EliminarActividad()
        {
            if (ActividadSeleccionada == null) return;

            try
            {
                LimpiarErrores();

                _actividadService.EliminarActividad(ActividadSeleccionada);
                RefrescarLista();
                LimpiarFormulario();
            }
            catch (InvalidOperationException ex)
            {
                ErrorActividad = ex.Message;
            }
        }

        /// <summary>
        /// Reinicia las propiedades del formulario a su estado inicial.
        /// </summary>
        private void LimpiarFormulario()
        {
            InputNombreActividad = "";
            InputAforoMaximo = "";
            LimpiarErrores();
            ActividadSeleccionada = null;
        }

        /// <summary> Evento para notificar cambios en las propiedades a la Vista. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> Invoca el evento PropertyChanged para una propiedad específica. </summary>
        /// <param name="propertyName">Nombre de la propiedad modificada.</param>
        protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}