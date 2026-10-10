namespace Lab07.Models
{
    public class ResumenCategoria
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public int CantidadProductos { get; set; }
        public int StockTotal { get; set; }
        public decimal PrecioPromedio { get; set; }
    }
}
