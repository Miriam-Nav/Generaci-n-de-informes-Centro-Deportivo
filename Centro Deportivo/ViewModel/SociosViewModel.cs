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
    public class SociosViewModel : INotifyPropertyChanged
    {
        private readonly SocioService _socioService = new SocioService();
        private readonly SocioRepositorio _repo = new SocioRepositorio();

        // LISTA DE SOCIOS 
        private List<Socio> _socios;
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
        public string InputNombre{ 
            get => _nombre; 
            set { 
                _nombre = value; 
                OnPropertyChanged(nameof(InputNombre)); 
            } 
        }

        private string _email;
        public string InputEmail { 
            get => _email; 
            set { 
                _email = value; 
                OnPropertyChanged(nameof(InputEmail)); 
            } 
        }

        private bool _activo;
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
        public string ErrorEmail { 
            get => _errorEmail; 
            set { 
                _errorEmail = value; 
                OnPropertyChanged(nameof(ErrorEmail)); 
            } 
        }

        // COMANDOS
        public ICommand CrearCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand GenerarInformeCommand { get; }

        public SociosViewModel()
        {
            RefrescarLista();

            CrearCommand = new RelayCommand(CrearSocio);
            ModificarCommand = new RelayCommand(ModificarSocio);
            EliminarCommand = new RelayCommand(EliminarSocio);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);
        }

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

        private void LimpiarFormulario()
        {
            InputNombre = ""; 
            InputEmail = ""; 
            EsActivo = false;
            ErrorNombre = "";
            ErrorEmail = "";
            SocioSeleccionado = null;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}