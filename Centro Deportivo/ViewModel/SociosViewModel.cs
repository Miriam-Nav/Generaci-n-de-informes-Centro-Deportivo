using Centro_Deportivo;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;


namespace ViewModel
{
    public class SociosViewModel : INotifyPropertyChanged
    {
        private CentroDeportivoEntities _db = new CentroDeportivoEntities();

        // LISTA DE SOCIOS
        private List<Socio> _socios;
        
        public List<Socio> Socios
        {
            // Devuelve la colección privada _socios
            get => _socios;

            // Se ejecuta cuando se asigna una nueva colección a Socios
            set
            {
                // Guarda el nuevo valor en el campo privado
                _socios = value;

                // Notifica que la propiedad ha cambiado y refresca el DataGrid y ComboBox
                OnPropertyChanged(nameof(Socios));
            }
        }

        // SOCIO SELECCIONADO
        private Socio _socioSeleccionado;
        public Socio SocioSeleccionado
        {
            get => _socioSeleccionado;

            // Se ejecuta cuando se selecciona una fila distinta en el DataGrid
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));

                // Rellena los campos del formulario con los datos del socio seleccionado 
                if (value != null)
                {
                    Nombre = value.Nombre;
                    Email = value.Email;
                    Activo = value.Activo;
                }
            }
        }

        // CAMPOS DEL FORMULARIO

        // Nombre
        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        // Email
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        // Activo
        private bool _activo;
        public bool Activo
        {
            get => _activo;
            set
            {
                _activo = value;
                OnPropertyChanged(nameof(Activo));
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

        // Error Email
        private string _errorEmail;
        public string ErrorEmail
        {
            get => _errorEmail;
            set
            {
                _errorEmail = value;
                OnPropertyChanged(nameof(ErrorEmail));
            }
        }

        // COMMANDS
        public ICommand CrearCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand ImprimirCommand { get; }
        public ICommand GenerarInformeCommand { get; }

        // CONSTRUCTOR
        public SociosViewModel()
        {
            try {
                // Carga los socios desde la BD
                var listaSocios = _db.Socio.ToList();

                // Pasa la lista a la propiedad pública Socios
                Socios = listaSocios;
            }
            catch (Exception ex)
            {
                ErrorEmail = $"Error al cargar los datos: {ex.Message}";
            }

            CrearCommand = new RelayCommand(CrearSocio);
            ModificarCommand = new RelayCommand(ModificarSocio);
            EliminarCommand = new RelayCommand(EliminarSocio);
            LimpiarCommand = new RelayCommand(LimpiarFormulario);
            GenerarInformeCommand = new RelayCommand(GenerarInforme);

        }
        

        // VALIDACIONES

        // Validación Nombre
        private bool ValidarNombre()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ErrorNombre = "El nombre no puede estar vacío";
                return false;
            }

            ErrorNombre = "";
            return true;
        }

        // Validación Email
        private bool ValidarEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorEmail = "El email no puede estar vacío";
                return false;
            }

            if (!Email.Contains("@"))
            {
                ErrorEmail = "El email no es válido";
                return false;
            }

            ErrorEmail = "";
            return true;
        }

        // Validación Email Crear
        private bool ValidarEmailDuplicadoCrear()
        {
            // Si se esta creando
          
            bool existe = _db.Socio.Any(socio => socio.Email == Email);
            if (existe)
            {
                ErrorEmail = "Ya existe un socio con este email";
                return false;
            }

            return true;


        }

        // Validación Email Modificar
        private bool ValidarEmailDuplicadoModificar()
        {

            // Si se esta modificando, se ignora el email del propio socio
            bool existe = _db.Socio.Any(socio => socio.Email == Email && socio.Id != SocioSeleccionado.Id);
            if (existe)
            {
                ErrorEmail = "Ya existe otro socio con este email";
                return false;
            }
            

            return true;
        }

        // Validación Form
        private bool ValidarFormulario(bool crear)
        {
            bool okNombre = ValidarNombre();
            bool okEmail = ValidarEmail();
            bool okDuplicado = false;

            // Si se selecciona crear valida el duplicado de crear
            if (crear)
            {
                okDuplicado = ValidarEmailDuplicadoCrear();
            }
            else {
                // Si se selecciona modificar valida el duplicado de modificar
                okDuplicado = ValidarEmailDuplicadoModificar();
            }

            return okNombre && okEmail && okDuplicado;
        }

        // CREAR SOCIO
        private void CrearSocio()
        {
            // Comprueba que todas las validaciones del formulario sean true
            bool resultado = ValidarFormulario(true);

            if (!resultado) { 
                return; 
            }
            try { 
                var nuevo = new Socio
                {
                    Nombre = Nombre,
                    Email = Email,
                    Activo = Activo
                };

                _db.Socio.Add(nuevo);
                _db.SaveChanges();

                Socios = _db.Socio.ToList();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorEmail = $"Error al guardar el socio: {ex.Message}";
            }
        }

        // MODIFICAR SOCIO
        private void ModificarSocio()
        {
            // Comprueba que hay socio seleccionado
            if (SocioSeleccionado == null) { 
                return; 
            }

            bool resultado = ValidarFormulario(false);

            if (!resultado) {
                return;
            }

            try { 
                SocioSeleccionado.Nombre = Nombre;
                SocioSeleccionado.Email = Email;
                SocioSeleccionado.Activo = Activo;

                _db.SaveChanges();

                Socios = _db.Socio.ToList();
            }
            catch (Exception ex)
            {
                ErrorEmail = $"Error al modificar el socio: {ex.Message}";
            }
        }

        // ELIMINAR SOCIO
        private void EliminarSocio()
        {
            // Comprueba que hay socio seleccionado
            if (SocioSeleccionado == null) { 
                return; 
            }

            try
            {
                // Validar si tiene reservas
                bool tieneReservas = _db.Reserva.Any(reserva => reserva.ActividadId == SocioSeleccionado.Id);

                if (tieneReservas)
                {
                    ErrorEmail = "No se puede eliminar: el socio tiene reservas en el historial.";
                    return;
                }
                _db.Socio.Remove(SocioSeleccionado);
                _db.SaveChanges();

                Socios = _db.Socio.ToList();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                ErrorEmail = $"Error al eliminar el socio: {ex.Message}";
            }
        }

        // LIMPIAR FORMULARIO
        private void LimpiarFormulario()
        {
            Nombre = "";
            Email = "";
            Activo = false;

            ErrorNombre = "";
            ErrorEmail = "";

            SocioSeleccionado = null;
        }

        private void GenerarInforme()
        {
            try
            {
                var miReporte = new crSocios();

                var datos = _db.Socio.ToList();

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

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
           PropertyChanged?.Invoke(this, new
           PropertyChangedEventArgs(propertyName));
        }
    }
}
