namespace Babel.Models;

public record DetailPesanan(Guid IdPesanan, int IdProduk, int JumlahProduk)
{
    public DetailPesanan() : this(Guid.Empty, 0, 0) {}
}