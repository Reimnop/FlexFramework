using System.Runtime.InteropServices;

namespace FlexFramework.Text;

/// <summary>
/// Represents a vertex in a text mesh.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TextVertex
{
    public float PositionX;
    public float PositionY;
    public float TextureCoordinateX;
    public float TextureCoordinateY;
    
    public TextVertex(float positionX, float positionY, float textureCoordinateX, float textureCoordinateY)
    {
        PositionX = positionX;
        PositionY = positionY;
        TextureCoordinateX = textureCoordinateX;
        TextureCoordinateY = textureCoordinateY;
    }
}