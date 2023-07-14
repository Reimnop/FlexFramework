using FlexFramework.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Data;

public interface ISamplerView : INamedObjectView
{
    FilterMode FilterMode { get; }
    WrapMode WrapMode { get; }
    Color4 BorderColor { get; }
    Hash128 Hash { get; }
}