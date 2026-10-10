using System.Windows;
using System.Windows.Controls;
using Lab07.Models;
using Lab07.Negocio;

namespace Lab07
{
    public partial class MainWindow : Window
    {
        private readonly ProductoNegocio _negocio = new ProductoNegocio();
        private int _idSeleccionado = 0;
        private bool _listo = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Categoria> categorias = _negocio.ListarCategorias();
                CmbCategoria.ItemsSource = categorias;

                var filtro = new List<Categoria>
                {
                    new Categoria { IdCategoria = 0, NombreCategoria = "Todas las categorías" }
                };
                filtro.AddRange(categorias);
                CmbFiltroCategoria.ItemsSource = filtro;
                CmbFiltroCategoria.SelectedIndex = 0;

                CmbOrden.ItemsSource = new[] { "Código", "Nombre", "Precio", "Stock" };
                CmbOrden.SelectedIndex = 0;

                _listo = true;
                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar a la base de datos:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarProductos()
        {
            int idCategoria = CmbFiltroCategoria.SelectedValue is int id ? id : 0;
            string orden = CmbOrden.SelectedItem as string ?? "Código";

            List<ProductoVista> lista = _negocio.ListarVista(TxtBuscar.Text, idCategoria, orden);
            DgProductos.ItemsSource = lista;
            TxtResultados.Text = lista.Count + " producto(s)";
        }

        private void LimpiarFormulario()
        {
            _idSeleccionado = 0;
            TxtId.Text = "Nuevo";
            TxtNombre.Clear();
            CmbCategoria.SelectedIndex = -1;
            TxtPrecio.Clear();
            TxtStock.Clear();
            DgProductos.SelectedItem = null;
            BtnEliminar.IsEnabled = false;
            TxtNombre.Focus();
        }

        private void FiltrosCambiaron(object sender, RoutedEventArgs e)
        {
            if (!_listo) return;
            try { CargarProductos(); }
            catch (Exception ex) { MostrarError(ex); }
        }

        private void BtnLimpiarBusqueda_Click(object sender, RoutedEventArgs e)
        {
            TxtBuscar.Clear();
            CmbFiltroCategoria.SelectedIndex = 0;
            CmbOrden.SelectedIndex = 0;
        }

        private void BtnResumen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ventana = new ResumenWindow(_negocio) { Owner = this };
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProductos.SelectedItem is ProductoVista p)
            {
                _idSeleccionado = p.IdProducto;
                TxtId.Text = p.IdProducto.ToString();
                TxtNombre.Text = p.NombreProducto;
                CmbCategoria.SelectedValue = p.IdCategoria;
                TxtPrecio.Text = p.PrecioUnidad.ToString("0");
                TxtStock.Text = p.UnidadesEnExistencia.ToString();
                BtnEliminar.IsEnabled = true;
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(TxtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio debe ser un número.", "Datos incorrectos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!short.TryParse(TxtStock.Text, out short stock))
            {
                MessageBox.Show("El stock debe ser un número entero.", "Datos incorrectos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var producto = new Producto
            {
                IdProducto = _idSeleccionado,
                NombreProducto = TxtNombre.Text,
                IdCategoria = CmbCategoria.SelectedValue is int id ? id : 0,
                PrecioUnidad = precio,
                UnidadesEnExistencia = stock
            };

            try
            {
                bool esNuevo = producto.IdProducto == 0;
                _negocio.Guardar(producto);
                CargarProductos();
                LimpiarFormulario();
                MessageBox.Show(esNuevo ? "Producto registrado." : "Producto actualizado.",
                    "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos incorrectos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_idSeleccionado == 0) return;

            var respuesta = MessageBox.Show("¿Eliminar el producto \"" + TxtNombre.Text + "\"?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                _negocio.Eliminar(_idSeleccionado);
                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private static void MostrarError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
