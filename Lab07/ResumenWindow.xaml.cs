using System.Windows;
using Lab07.Negocio;

namespace Lab07
{
    public partial class ResumenWindow : Window
    {
        public ResumenWindow(ProductoNegocio negocio)
        {
            InitializeComponent();
            DgResumen.ItemsSource = negocio.ResumenPorCategoria();
        }
    }
}
