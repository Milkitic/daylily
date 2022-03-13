using System.Security.Cryptography;
using System.Text;

namespace daylily.Utils;

public static class EncryptUtil
{
    public static string EncryptAes256UseMd5(string sourceStr, string? key = null, string? iv = null)
    {
        Span<byte> keySpan = stackalloc byte[32];
        FillSpan(key, keySpan);
        Span<byte> ivSpan = stackalloc byte[16];
        FillSpan(iv, ivSpan);

        using var aes = GetAesProvider(keySpan, ivSpan);
        using var crypto = aes.CreateEncryptor();

        var bytes = Encoding.UTF8.GetBytes(sourceStr);
        byte[] encrypted = crypto.TransformFinalBlock(bytes, 0, bytes.Length);

        return Convert.ToBase64String(encrypted);
    }

    public static string DecryptAes256UseMd5(string encryptedBase64, string? key = null, string? iv = null)
    {
        return DecryptAes256UseMd5(Convert.FromBase64String(encryptedBase64), key, iv);
    }

    public static string DecryptAes256UseMd5(byte[] encryptedBytes, string? key = null, string? iv = null)
    {
        Span<byte> keySpan = stackalloc byte[32];
        FillSpan(key, keySpan);
        Span<byte> ivSpan = stackalloc byte[16];
        FillSpan(iv, ivSpan);

        using var aes = GetAesProvider(keySpan, ivSpan);
        using var crypto = aes.CreateEncryptor();

        byte[] decrypted = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    private static Aes GetAesProvider(Span<byte> keySpan, Span<byte> ivSpan)
    {
        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.Key = keySpan.ToArray();
        aes.IV = ivSpan.ToArray();
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        return aes;
    }

    private static void FillSpan(string? content, Span<byte> span)
    {
        if (content == null) return;
        using var md5 = MD5.Create();
        var md5Byte = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
        md5Byte.CopyTo(span);
    }
}