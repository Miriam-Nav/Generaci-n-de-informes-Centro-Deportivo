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
    public class ActividadesViewModel : INotifyPropertyChanged
    {
        private readonly ActividadesService _actividadService = new ActividadesService();
        private readonly ActividadRepositorio _repo = new ActividadRepositorio();

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
                    InputNombreActividad = value.Nombre;
                    InputAforoMaximo = value.AforoMaximo.ToString();
                }
            }
        }

        // CAMPOS DEL FORMULARIO

        // Nombre Actividad
        private string _nombreActividad;
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
            RefrescarLista();
            CrearCommand = new RelayCommand(CrearActividad);
            ModificarCommand = new RelayCommand(ModificarActividad);
            EliminarCommand = new RelayCommand(EliminarActividad);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
        }

        private void RefrescarLista()
        {
            Actividades = _repo.Seleccionar();
        }

        private void LimpiarErrores() {

                ErrorNombre = "";

                ErrorAforo = "";
           
                ErrorActividad = "";
        }

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

        private void LimpiarFormulario()
        {
            InputNombreActividad = "";
            InputAforoMaximo = "";
            LimpiarErrores();
            ActividadSeleccionada = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}