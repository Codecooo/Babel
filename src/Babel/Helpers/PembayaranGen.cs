using Babel.Models;

namespace Babel.Helpers;

public static class PembayaranGen
{
    public static decimal GetTotalPembayaran(Pesanan pesanan, List<DetailPesanan> listDetailPesanan, List<Produk> listProduk)
    {
        var produkLookup = listProduk.ToDictionary(p => p.IdProduk, p => p.HargaPerUnit);
        var total = 0m;

        foreach (var detail in listDetailPesanan.Where(d => d.IdPesanan == pesanan.IdPesanan))
        {
            if (produkLookup.TryGetValue(detail.IdProduk, out var harga))
            {
                total += harga * detail.JumlahProduk;
            }
        }

        return total;
    }

    public static StatusPembayaran GenerateStatusPembayaran(Pesanan pesanan)
    {
        return pesanan.StatusPesanan switch
        {
            StatusPesanan.Batal => StatusPembayaran.Batal,
            StatusPesanan.MenungguPembayaran => StatusPembayaran.MenungguPembayaran,
            _ => StatusPembayaran.Berhasil
        };
    }
}