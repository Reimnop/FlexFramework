using System.Text;

namespace FlexFramework.Util;

public unsafe struct Hash128
{
    public const int Length = 16;
    
    private fixed byte data[Length];

    public Hash128(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length != Length)
            throw new ArgumentException($"Buffer must be {Length} bytes long!", nameof(buffer));

        fixed (byte* ptr = data)
        {
            buffer.CopyTo(new Span<byte>(ptr, Length));
        }
    }
    
    public static Hash128 operator ^(Hash128 left, Hash128 right)
    {
        Span<byte> result = stackalloc byte[Length];
        for (int i = 0; i < Length; i++)
        {
            result[i] = (byte)(left[i] ^ right[i]);
        }
        return new Hash128(result);
    }

    public override int GetHashCode()
    {
        // Return hash code
        HashCode hashCode = new();
        for (int i = 0; i < Length; i++)
        {
            hashCode.Add(data[i]);
        }
        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        // Return hex string
        StringBuilder sb = new StringBuilder(Length * 2);
        for (int i = 0; i < Length; i++)
        {
            sb.Append(data[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            
            return data[index];
        }
        set
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException(nameof(index));
            
            data[index] = value;
        }
    }
}