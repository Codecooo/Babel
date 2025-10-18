namespace Babel.Models;

public record Pelanggan(Guid IdPelanggan, string Nama, string Email, string NoHp, string Alamat)
{
    public Pelanggan() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty) {}
}