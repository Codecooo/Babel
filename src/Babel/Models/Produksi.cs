namespace Babel.Models;

public record Produksi(int IdProduksi, Guid IdKaryawan, DateOnly TanggalProduksi, string Keterangan);