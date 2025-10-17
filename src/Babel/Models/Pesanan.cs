namespace Babel.Models;

public record Pesanan(Guid IdPesanan, Guid IdPelanggan, Guid IdKaryawan, int IdProduksi, DateTime TanggalPesanan, StatusPesanan StatusPesanan);