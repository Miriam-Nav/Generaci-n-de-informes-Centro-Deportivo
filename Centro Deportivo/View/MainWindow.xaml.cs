using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViewModel;

namespace View
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new MainWindowViewModel();
            this.DataContext = vm;

            vm.OnAccionSolicitada += Vm_OnAccionSolicitada;
        }

        private void Vm_OnAccionSolicitada()
        {
            switch (vm.AccionSolicitada)
            {
                case "Reservas":
                    new ReservasView().Show();
                    break;

                case "Socios":
                    new SociosView().Show();
                    break;

                case "Actividades":
                    new ActividadesView().Show();
                    break;
            }
        }
    }

}
