namespace Babel.Models;

public record PemakaianMesin(int IdProduksi, int IdMesin, DateTime WaktuPemakaian)
{
    public PemakaianMesin() : this(0, 0, DateTime.Now) {}
}