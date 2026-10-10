using Lab07.Datos;
using Lab07.Models;

namespace Lab07.Negocio
{
    public class ProductoNegocio
    {
        private readonly AccesoDatos _datos = new AccesoDatos();

        public List<Producto> Listar()
        {
            return _datos.ListarProductos();
        }

        public List<Producto> Buscar(string texto)
        {
            texto = (texto ?? string.Empty).Trim();
            return texto.Length == 0 ? _datos.ListarProductos() : _datos.BuscarProductos(texto);
        }

        public List<Categoria> ListarCategorias()
        {
            return _datos.ListarCategorias();
        }

        public void Guardar(Producto p)
        {
            Validar(p);

            if (p.IdProducto == 0)
            {
                _datos.InsertarProducto(p);
            }
            else
            {
                int filas = _datos.ActualizarProducto(p);
                if (filas == 0)
                {
                    throw new InvalidOperationException("El producto ya no existe o fue eliminado.");
                }
            }
        }

        public void Eliminar(int idProducto)
        {
            if (idProducto <= 0)
            {
                throw new InvalidOperationException("Selecciona un producto de la lista para eliminarlo.");
            }

            int filas = _datos.EliminarProductoLogico(idProducto);
            if (filas == 0)
            {
                throw new InvalidOperationException("No se encontró el producto a eliminar.");
            }
        }

        private void Validar(Producto p)
        {
            p.NombreProducto = (p.NombreProducto ?? string.Empty).Trim();

            if (p.NombreProducto.Length == 0)
            {
                throw new ArgumentException("El nombre del producto es obligatorio.");
            }
            if (p.NombreProducto.Length > 40)
            {
                throw new ArgumentException("El nombre no puede pasar de 40 caracteres.");
            }
            if (p.IdCategoria <= 0)
            {
                throw new ArgumentException("Elige una categoría.");
            }
            if (p.PrecioUnidad < 0)
            {
                throw new ArgumentException("El precio no puede ser negativo.");
            }
            if (p.UnidadesEnExistencia < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo.");
            }

            bool repetido = _datos.ListarProductos().Any(x =>
                x.IdProducto != p.IdProducto &&
                string.Equals(x.NombreProducto, p.NombreProducto, StringComparison.OrdinalIgnoreCase));
            if (repetido)
            {
                throw new ArgumentException("Ya existe un producto con ese nombre.");
            }
        }
    }
}
