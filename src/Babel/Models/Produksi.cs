namespace Babel.Models;

public record Produksi(int IdProduksi, Guid IdKaryawan, DateOnly TanggalProduksi, string Keterangan)
{
    public Produksi() : this(default, Guid.Empty, default, string.Empty) {}
}