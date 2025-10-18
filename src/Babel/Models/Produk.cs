namespace Babel.Models;

public record Produk(int IdProduk, string NamaProduk, string Ukuran, JenisProduk JenisProduk, decimal HargaPerUnit)
{
    public Produk() : this (0, string.Empty, string.Empty, JenisProduk.Cetak, 0) {}
}