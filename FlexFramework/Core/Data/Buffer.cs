using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FlexFramework.Util;

namespace FlexFramework.Core.Data;

public class Buffer
{
    private struct ReadOnlyBuffer : IBufferView
    {
        public ReadOnlySpan<byte> Data => buffer.Data;
        public int Size => buffer.Size;
        public Hash128 Hash => buffer.Hash;
        
        private readonly Buffer buffer;
        
        public ReadOnlyBuffer(Buffer buffer)
        {
            this.buffer = buffer;
        }
    }
    
    public ReadOnlySpan<byte> Data => new(data, 0, size);
    public int Size => size;

    public Hash128 Hash
    {
        get
        {
            if (shouldUpdateHash)
            {
                shouldUpdateHash = false;
                hash = HashUtil.Hash(data);
            }
            
            return hash;
        }
    }
    public IBufferView ReadOnly => new ReadOnlyBuffer(this);
    
    private byte[] data;
    private int size = 0;
    private Hash128 hash;
    
    private bool readOnly = false;
    private bool shouldUpdateHash = true; // Used to avoid updating hash when it's not necessary
    
    public Buffer(int capacity = 1024)
    {
        data = new byte[capacity];
        SetUpdateHash();
    }

    private void SetUpdateHash()
    { 
        shouldUpdateHash = true;
    }
    
    public Buffer SetReadOnly()
    {
        readOnly = true;
        return this;
    }

    public unsafe Buffer Append<T>(T data) where T : unmanaged
    {
        if (readOnly)
            throw new InvalidOperationException("Cannot modify read-only buffer!");

        var length = sizeof(T); // We don't need Unsafe.SizeOf because this method is already unsafe
        var offset = size;

        // Resize buffer if necessary
        if (offset + length > this.data.Length)
        {
            var newSize = this.data.Length;
            while (offset + length > newSize)
                newSize *= 2;

            Array.Resize(ref this.data, newSize);
        }
        
        // Copy data
        fixed (byte* ptr = this.data)
            Unsafe.CopyBlock(ptr + offset, &data, (uint) length);

        // Update size
        size += length;
        
        // Recalculate hash
        SetUpdateHash();
        
        return this;
    }

    public Buffer SetData(IntPtr ptr, int length)
    {
        if (readOnly)
            throw new InvalidOperationException("Cannot modify read-only buffer!");
        
        if (length > data.Length)
            Array.Resize(ref data, length);

        Marshal.Copy(ptr, data, 0, length);
        size = length;
        
        SetUpdateHash();
        
        return this;
    }
    
    public unsafe Buffer SetData<T>(ReadOnlySpan<T> buffer) where T : unmanaged
    {
        var length = buffer.Length * sizeof(T);

        if (length > 0)
            fixed (T* ptr = buffer)
                return SetData((IntPtr) ptr, length);

        return Clear();
    }
    
    public Buffer Clear()
    {
        if (readOnly)
            throw new InvalidOperationException("Cannot modify read-only buffer!");
        
        size = 0;
        SetUpdateHash();
        
        return this;
    }
}