using FlexFramework.Util;

namespace FlexFramework.Core.Data;

public interface IBufferView
{
    ReadOnlySpan<byte> Data { get; }
    int Size { get; }
    Hash128 Hash { get; }
}