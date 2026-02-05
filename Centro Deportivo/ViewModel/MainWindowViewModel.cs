using Centro_Deportivo;
using System;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// ViewModel principal de la aplicación. 
    /// Se encarga de la navegación entre las distintas secciones del Centro Deportivo: 
    /// Reservas, Socios y Actividades.
    /// </summary>
    public class MainWindowViewModel
    {
        /// <summary> Comando para navegar a la sección de gestión de reservas. </summary>
        public ICommand AbrirReservasCommand { get; }

        /// <summary> Comando para navegar a la sección de gestión de socios. </summary>
        public ICommand AbrirSociosCommand { get; }

        /// <summary> Comando para navegar a la sección de gestión de actividades. </summary>
        public ICommand AbrirActividadesCommand { get; }

        /// <summary> 
        /// Almacena el nombre de la acción o vista que se ha solicitado abrir. 
        /// </summary>
        public string AccionSolicitada { get; private set; }

        /// <summary>
        /// Constructor del ViewModel principal. 
        /// Inicializa los comandos de navegación vinculándolos a la lógica de cambio de vista.
        /// </summary>
        public MainWindowViewModel()
        {
            AbrirReservasCommand = new RelayCommand(() => SolicitarAccion("Reservas"));
            AbrirSociosCommand = new RelayCommand(() => SolicitarAccion("Socios"));
            AbrirActividadesCommand = new RelayCommand(() => SolicitarAccion("Actividades"));
        }

        /// <summary>
        /// Método privado que gestiona la lógica de solicitud de cambio de pantalla.
        /// Actualiza la propiedad <see cref="AccionSolicitada"/> y dispara el evento de notificación.
        /// </summary>
        /// <param name="accion">Nombre de la vista de destino (ej: "Socios").</param>
        private void SolicitarAccion(string accion)
        {
            AccionSolicitada = accion;
            OnAccionSolicitada?.Invoke();
        }

        /// <summary>
        /// Evento que notifica a la Vista (Code-behind) que se ha solicitado un cambio de sección.
        /// </summary>
        public event Action OnAccionSolicitada;
    }


}
