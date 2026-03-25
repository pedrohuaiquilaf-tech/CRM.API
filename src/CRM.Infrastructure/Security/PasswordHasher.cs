namespace CRM.Infrastructure.Security;

using System.Security.Cryptography;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var combined = new byte[48];
        Buffer.BlockCopy(salt, 0, combined, 0, 16);
        Buffer.BlockCopy(hash, 0, combined, 16, 32);
        return Convert.ToBase64String(combined);
    }

    public static bool Verify(string password, string storedHash)
    {
        byte[] combined;
        try { combined = Convert.FromBase64String(storedHash); }
        catch { return false; }

        if (combined.Length != 48)
            return false;

        var salt = new byte[16];
        Buffer.BlockCopy(combined, 0, salt, 0, 16);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
            if (combined[i + 16] != hash[i])
                return false;

        return true;
    }
}
