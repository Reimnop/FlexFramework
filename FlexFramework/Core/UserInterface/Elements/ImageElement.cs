using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public enum ImageMode
{
    Fill,
    Fit,
    Stretch
}

public class ImageElement : Element, IRenderable
{
    public ImageMode ImageMode { get; set; } = ImageMode.Fill;

    private Box2 bounds;
    private readonly ImageEntity imageEntity;

    public ImageElement(TextureSampler texture)
    {
        imageEntity = new ImageEntity(texture);
    }
    
    protected override void UpdateLayout(Box2 bounds)
    {
        this.bounds = bounds;
    }

    public void Render(RenderArgs args)
    {
        var texture = imageEntity.Texture;
        var size = bounds.Size;
        var matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(0.5f, 0.5f, 0.0f);
        switch (ImageMode)
        {
            case ImageMode.Fill:
                if (size.X / size.Y > texture.Width / (float) texture.Height)
                {
                    matrixStack.Scale(size.X, size.X * texture.Height / texture.Width, 1.0f);
                }
                else
                {
                    matrixStack.Scale(size.Y * texture.Width / texture.Height, size.Y, 1.0f);
                }
                break;
            case ImageMode.Fit:
                if (size.X / size.Y > texture.Width / (float) texture.Height)
                {
                    matrixStack.Scale(size.Y * texture.Width / texture.Height, size.Y, 1.0f);
                }
                else
                {
                    matrixStack.Scale(size.X, size.X * texture.Height / texture.Width, 1.0f);
                }
                break;
            case ImageMode.Stretch:
                matrixStack.Scale(size.X, size.Y, 1.0f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        matrixStack.Translate(bounds.Min.X, bounds.Min.Y, 0.0f);
        imageEntity.Render(args);
        matrixStack.Pop();
    }
}