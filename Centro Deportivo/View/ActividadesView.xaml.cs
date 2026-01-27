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
    /// Lógica de interacción para ActividadesView.xaml
    /// </summary>
    public partial class ActividadesView : Window
    {
        public ActividadesView()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.ActividadesViewModel();
        }
    }
}
