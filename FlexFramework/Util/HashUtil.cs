using System.Security.Cryptography;

namespace FlexFramework.Util;

public static class HashUtil
{
    public static Hash128 Hash(ReadOnlySpan<byte> buffer)
    {
        // MD5 hash
        Span<byte> result = stackalloc byte[Hash128.Length];
        if (MD5.TryHashData(buffer, result, out _))
        {
            return new Hash128(result);
        }
        
        throw new Exception("Failed to hash data!");
    }
}