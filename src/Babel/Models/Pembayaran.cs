namespace Babel.Models;

public record Pembayaran(
    Guid IdPembayaran,
    Guid IdPesanan,
    MetodePembayaran MetodePembayaran,
    DateTime TanggalPembayaran,
    StatusPembayaran StatusPembayaran,
    decimal TotalPembayaran)
{
    public Pembayaran() : this(Guid.Empty, Guid.Empty, MetodePembayaran.Cash, DateTime.Now, StatusPembayaran.MenungguPembayaran, 0) {}
}