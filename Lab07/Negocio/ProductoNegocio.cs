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

        /*
         Consulta LINQ principal de la lista:
         1. Inner join entre productos y categorias para mostrar el nombre de la categoria.
         2. Filtro (Where) por categoria cuando se elige una.
         3. Ordenamiento (OrderBy) segun el criterio elegido.
        */
        public List<ProductoVista> ListarVista(string texto, int idCategoria, string orden)
        {
            List<Producto> productos = Buscar(texto);
            List<Categoria> categorias = _datos.ListarCategorias();

            IEnumerable<ProductoVista> consulta =
                from p in productos
                join c in categorias on p.IdCategoria equals c.IdCategoria
                select new ProductoVista
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = c.NombreCategoria,
                    PrecioUnidad = p.PrecioUnidad,
                    UnidadesEnExistencia = p.UnidadesEnExistencia
                };

            if (idCategoria > 0)
            {
                consulta = consulta.Where(v => v.IdCategoria == idCategoria);
            }

            IEnumerable<ProductoVista> ordenada = orden switch
            {
                "Nombre" => consulta.OrderBy(v => v.NombreProducto),
                "Precio" => consulta.OrderBy(v => v.PrecioUnidad),
                "Stock" => consulta.OrderBy(v => v.UnidadesEnExistencia),
                _ => consulta.OrderBy(v => v.IdProducto)
            };

            return ordenada.ToList();
        }

        /*
         Consulta LINQ con inner join y group by:
         agrupa los productos activos por categoria y calcula cantidad, stock total y precio promedio.
        */
        public List<ResumenCategoria> ResumenPorCategoria()
        {
            List<Producto> productos = _datos.ListarProductos();
            List<Categoria> categorias = _datos.ListarCategorias();

            var resumen =
                from p in productos
                join c in categorias on p.IdCategoria equals c.IdCategoria
                group p by c.NombreCategoria into g
                orderby g.Key
                select new ResumenCategoria
                {
                    NombreCategoria = g.Key,
                    CantidadProductos = g.Count(),
                    StockTotal = g.Sum(x => (int)x.UnidadesEnExistencia),
                    PrecioPromedio = Math.Round(g.Average(x => x.PrecioUnidad), 2)
                };

            return resumen.ToList();
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
