using System.Security.Cryptography;

namespace Billing.Service.Infrastructure.Security;

public static class AesGcmEncryption
{
    public static byte[] DecryptBytes(byte[] payload, byte[] key)
    {
        if (payload.Length < 12 + 16)
        {
            throw new CryptographicException("Payload cifrado inválido.");
        }

        var nonce = payload.AsSpan(0, 12).ToArray();
        var tag = payload.AsSpan(12, 16).ToArray();
        var cipherText = payload.AsSpan(28).ToArray();
        var plainText = new byte[cipherText.Length];

        using var aes = new AesGcm(key, tagSizeInBytes: 16);
        aes.Decrypt(nonce, cipherText, tag, plainText);
        return plainText;
    }

    public static string DecryptString(string payloadBase64, byte[] key)
    {
        var payload = Convert.FromBase64String(payloadBase64);
        var plainBytes = DecryptBytes(payload, key);
        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}
