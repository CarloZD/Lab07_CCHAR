namespace Lab07.Models
{
    public class ProductoVista
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public decimal PrecioUnidad { get; set; }
        public short UnidadesEnExistencia { get; set; }
    }
}
