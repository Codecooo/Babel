namespace Babel.Models;

public record Pembayaran(Guid IdPembayaran, Guid IdPesanan, MetodePembayaran MetodePembayaran, DateTime TanggalPembayaran, StatusPembayaran StatusPembayaran, decimal TotalPembayaran);