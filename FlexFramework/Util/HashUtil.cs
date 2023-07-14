using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

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
    
    public static Hash128 Hash(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        return Hash(buffer);
    }
    
    public static Hash128 Hash(long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        return Hash(buffer);
    }
    
    public static Hash128 Hash(float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        return Hash(buffer);
    }
    
    public static Hash128 Hash(double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        return Hash(buffer);
    }
    
    public static Hash128 Hash(string value)
    {
        return Hash(Encoding.UTF8.GetBytes(value));
    }
    
    public static Hash128 Hash<T>(T value) where T : unmanaged
    {
        Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
        return Hash(buffer);
    }
}