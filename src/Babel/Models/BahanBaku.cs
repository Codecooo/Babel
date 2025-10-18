namespace Babel.Models;

public record BahanBaku(int IdBahan, string NamaBahan, JenisBahan JenisBahan, int StokBahan)
{
    public BahanBaku() : this(0, string.Empty, JenisBahan.HVS, 0) {}
}