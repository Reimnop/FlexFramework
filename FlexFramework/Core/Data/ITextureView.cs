namespace FlexFramework.Core.Data;

public interface ITextureView : INamedObjectView
{
    int Width { get; }
    int Height { get; }
    PixelFormat Format { get; }
    IBufferView Data { get; }
}