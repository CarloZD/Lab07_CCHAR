using System.Data;
using Lab07.Models;
using Microsoft.Data.SqlClient;

namespace Lab07.Datos
{
    public class AccesoDatos
    {
        private const string CadenaConexion =
            "Server=(localdb)\\MSSQLLocalDB;Database=Neptuno;Trusted_Connection=True;TrustServerCertificate=True;";

        public List<Producto> ListarProductos()
        {
            var lista = new List<Producto>();

            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_ListarProductos", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(MapearProducto(lector));
            }
            return lista;
        }

        public List<Producto> BuscarProductos(string texto)
        {
            var lista = new List<Producto>();

            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_BuscarProductos", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@texto", SqlDbType.VarChar, 40).Value = texto ?? string.Empty;

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(MapearProducto(lector));
            }
            return lista;
        }

        public Producto? ObtenerProductoPorId(int idProducto)
        {
            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_ObtenerProductoPorId", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idproducto", SqlDbType.Int).Value = idProducto;

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            return lector.Read() ? MapearProducto(lector) : null;
        }

        public List<Categoria> ListarCategorias()
        {
            var lista = new List<Categoria>();

            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_ListarCategoriasCombo", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Categoria
                {
                    IdCategoria = lector.GetInt32(lector.GetOrdinal("idcategoria")),
                    NombreCategoria = ObtenerTexto(lector, "nombrecategoria")
                });
            }
            return lista;
        }

        public int InsertarProducto(Producto p)
        {
            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_InsProductos", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            AgregarParametrosComunes(comando, p);

            conexion.Open();
            return comando.ExecuteNonQuery();
        }

        public int ActualizarProducto(Producto p)
        {
            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_UpdProductos", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idproducto", SqlDbType.Int).Value = p.IdProducto;
            AgregarParametrosComunes(comando, p);

            conexion.Open();
            return comando.ExecuteNonQuery();
        }

        public int EliminarProductoLogico(int idProducto)
        {
            using var conexion = new SqlConnection(CadenaConexion);
            using var comando = new SqlCommand("dbo.USP_DelProductosLogico", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idproducto", SqlDbType.Int).Value = idProducto;

            conexion.Open();
            return comando.ExecuteNonQuery();
        }

        private static void AgregarParametrosComunes(SqlCommand comando, Producto p)
        {
            comando.Parameters.Add("@nombreProducto", SqlDbType.VarChar, 40).Value = p.NombreProducto;
            comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = p.IdCategoria;

            var precio = comando.Parameters.Add("@precioUnidad", SqlDbType.Decimal);
            precio.Precision = 18;
            precio.Scale = 0;
            precio.Value = p.PrecioUnidad;

            comando.Parameters.Add("@unidadesEnExistencia", SqlDbType.SmallInt).Value = p.UnidadesEnExistencia;
        }

        private static Producto MapearProducto(SqlDataReader lector)
        {
            int iCategoria = lector.GetOrdinal("idCategoria");
            int iPrecio = lector.GetOrdinal("precioUnidad");
            int iStock = lector.GetOrdinal("unidadesEnExistencia");
            int iActivo = lector.GetOrdinal("Activo");

            return new Producto
            {
                IdProducto = lector.GetInt32(lector.GetOrdinal("idproducto")),
                NombreProducto = ObtenerTexto(lector, "nombreProducto"),
                IdCategoria = lector.IsDBNull(iCategoria) ? 0 : lector.GetInt32(iCategoria),
                PrecioUnidad = lector.IsDBNull(iPrecio) ? 0 : lector.GetDecimal(iPrecio),
                UnidadesEnExistencia = lector.IsDBNull(iStock) ? (short)0 : lector.GetInt16(iStock),
                Activo = lector.IsDBNull(iActivo) || lector.GetBoolean(iActivo)
            };
        }

        private static string ObtenerTexto(SqlDataReader lector, string columna)
        {
            int indice = lector.GetOrdinal(columna);
            return lector.IsDBNull(indice) ? string.Empty : lector.GetString(indice);
        }
    }
}
