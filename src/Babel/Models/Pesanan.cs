namespace Babel.Models;

public record Pesanan(
    Guid IdPesanan,
    Guid IdPelanggan,
    Guid IdKaryawan,
    int IdProduksi,
    DateTime TanggalPesanan,
    StatusPesanan StatusPesanan)
{
    public Pesanan() : this(Guid.Empty, Guid.Empty, Guid.Empty, 0, default, default) {}
}