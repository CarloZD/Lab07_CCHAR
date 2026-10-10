namespace Lab07.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
        public decimal PrecioUnidad { get; set; }
        public short UnidadesEnExistencia { get; set; }
        public bool Activo { get; set; } = true;
    }
}
