using Centro_Deportivo;
using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel encargado de la gestión de socios del centro deportivo.
    /// Controla la lógica de alta, modificación y baja, así como la validación de sus datos de contacto.
    /// </summary>
    public class SociosViewModel : INotifyPropertyChanged
    {
        private readonly SocioService _socioService = new SocioService();
        private readonly SocioRepositorio _repo = new SocioRepositorio();

        // LISTA DE SOCIOS 
        private List<Socio> _socios;
        /// <summary> Lista completa de socios registrados en el sistema. </summary>
        public List<Socio> Socios
        {
            get => _socios;
            set { 
                _socios = value; 
                OnPropertyChanged(nameof(Socios)); 
            }
        }

        // SOCIO SELECCIONADO
        private Socio _socioSeleccionado;
        /// <summary> Socio seleccionado actualmente en el listado. Actualiza los campos del formulario al cambiar. </summary>
        public Socio SocioSeleccionado
        {
            get => _socioSeleccionado;
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));

                if (value != null)
                {
                    InputNombre = value.Nombre;
                    InputEmail = value.Email;
                    EsActivo = value.Activo;
                }
            }
        }

        // CAMPOS DEL FORMULARIO
        private string _nombre;
        /// <summary> Nombre completo del socio introducido en el formulario. </summary>
        public string InputNombre{ 
            get => _nombre; 
            set { 
                _nombre = value; 
                OnPropertyChanged(nameof(InputNombre)); 
            } 
        }

        private string _email;
        /// <summary> Dirección de correo electrónico del socio introducida en el formulario. </summary>
        public string InputEmail { 
            get => _email; 
            set { 
                _email = value; 
                OnPropertyChanged(nameof(InputEmail)); 
            } 
        }

        private bool _activo;
        /// <summary> Indica si el socio se encuentra en estado activo para realizar reservas. </summary>
        public bool EsActivo { 
            get => _activo; 
            set { 
                _activo = value; 
                OnPropertyChanged(nameof(EsActivo)); 
            } 
        }

        // MENSAJES DE ERROR
        // Error Nombre
        private string _errorNombre;
        /// <summary> Almacena el mensaje de error relacionado al nombre del socio. </summary>
        public string ErrorNombre
        {
            get => _errorNombre;
            set
            {
                _errorNombre = value;
                OnPropertyChanged(nameof(ErrorNombre));
            }
        }

        private string _errorEmail;
        /// <summary> Almacena el mensaje de error relacionado a la validación del correo electrónico. </summary>
        public string ErrorEmail { 
            get => _errorEmail; 
            set { 
                _errorEmail = value; 
                OnPropertyChanged(nameof(ErrorEmail)); 
            } 
        }

        // COMMANDS
        /// <summary> Comando para registrar un nuevo socio en el sistema. </summary>
        public ICommand CrearCommand { get; }

        /// <summary> Comando para actualizar la información del socio seleccionado. </summary>
        public ICommand ModificarCommand { get; }

        /// <summary> Comando para eliminar al socio seleccionado del sistema. </summary>
        public ICommand EliminarCommand { get; }

        /// <summary> Comando para resetear los campos del formulario y limpiar los mensajes de error. </summary>
        public ICommand LimpiarCommand { get; }

        /// <summary> Comando para generar un informe con el listado completo de socios. </summary>
        public ICommand GenerarInformeCommand { get; }

        // CONSTRUCTOR
        /// <summary>
        /// Constructor del ViewModel de Socios.
        /// Carga la lista inicial y vincula los comandos a sus métodos.
        /// </summary>
        public SociosViewModel()
        {
            RefrescarLista();

            CrearCommand = new RelayCommand(CrearSocio);
            ModificarCommand = new RelayCommand(ModificarSocio);
            EliminarCommand = new RelayCommand(EliminarSocio);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);
        }

        /// <summary>
        /// Actualiza la colección de socios consultando al repositorio.
        /// </summary>
        private void RefrescarLista()
        {
            try
            {
                Socios = _repo.Seleccionar();
            }
            catch (Exception ex)
            {
                ErrorEmail = $"Error al cargar los datos: {ex.Message}";
            }
        }

        /// <summary>
        /// Crea un nuevo socio y gestiona posibles excepciones de validación.
        /// </summary>
        private void CrearSocio()
        {
            try
            {
                ErrorEmail = "";

                // Añade los datos de la Base de Datos
                var nuevo = new Socio { Nombre = InputNombre, Email = InputEmail, Activo = EsActivo };

                _socioService.CrearSocio(nuevo);

                RefrescarLista();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                ErrorEmail = ex.Message;
            }
        }

        /// <summary>
        /// Modifica los datos del socio seleccionado y gestiona posibles excepciones.
        /// </summary>
        private void ModificarSocio()
        {
            if (SocioSeleccionado == null) return;

            try
            {
                ErrorEmail = "";
                SocioSeleccionado.Nombre = InputNombre;
                SocioSeleccionado.Email = InputEmail;
                SocioSeleccionado.Activo = EsActivo;

                _socioService.ActualizarSocio(SocioSeleccionado);

                RefrescarLista();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                ErrorEmail = ex.Message;
            }
        }

        /// <summary>
        /// Gestiona la eliminación de un socio, capturando errores de integridad o restricciones.
        /// </summary>
        private void EliminarSocio()
        {
            if (SocioSeleccionado == null)
            { 
                return;
            }

            try
            {
                _socioService.EliminarSocio(SocioSeleccionado);
                RefrescarLista();
                LimpiarFormulario();
            }
            catch (InvalidOperationException ex)
            {
                ErrorEmail = ex.Message;
            }
        }

        /// <summary>
        /// Reinicia las propiedades del formulario de socios a su estado inicial.
        /// </summary>
        private void LimpiarFormulario()
        {
            InputNombre = ""; 
            InputEmail = ""; 
            EsActivo = false;
            ErrorNombre = "";
            ErrorEmail = "";
            SocioSeleccionado = null;
        }

        /// <summary>
        /// Genera y muestra un informe visual con el listado de socios.
        /// </summary>
        private void GenerarInforme()
        {
            try
            {
                var miReporte = new crSocios();

                var datos = _repo.Seleccionar();

                miReporte.SetDataSource(datos);

                var ventanaVisor = new InformesView();

                ventanaVisor.reportViewer.ViewerCore.ReportSource = miReporte;

                ventanaVisor.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al generar: {ex.Message}");
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