using Centro_Deportivo;
using System;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand AbrirReservasCommand { get; }
        public ICommand AbrirSociosCommand { get; }
        public ICommand AbrirActividadesCommand { get; }

        public string AccionSolicitada { get; private set; }

        public MainWindowViewModel()
        {
            AbrirReservasCommand = new RelayCommand(() => SolicitarAccion("Reservas"));
            AbrirSociosCommand = new RelayCommand(() => SolicitarAccion("Socios"));
            AbrirActividadesCommand = new RelayCommand(() => SolicitarAccion("Actividades"));
        }

        private void SolicitarAccion(string accion)
        {
            AccionSolicitada = accion;
            OnAccionSolicitada?.Invoke();
        }

        public event Action OnAccionSolicitada;
    }


}
