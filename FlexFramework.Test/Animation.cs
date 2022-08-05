using FlexFramework.Rendering.Data;
using OpenTK.Graphics.OpenGL4;

namespace FlexFramework.Test;

public class Animation : IDisposable
{
    public List<Texture2D> AnimationFrames { get; }
    public double Framerate { get; }

    public Animation(double framerate)
    {
        Framerate = framerate;

        AnimationFrames = new List<Texture2D>();
    }

    public void LoadFrame(string path)
    {
        Texture2D texture2D = Texture2D.FromFile("idk", path);
        texture2D.SetMagFilter(TextureMagFilter.Linear);
        texture2D.SetMinFilter(TextureMinFilter.Linear);
        
        AnimationFrames.Add(texture2D);
    }

    public void Dispose()
    {
        foreach (Texture2D texture2D in AnimationFrames)
        {
            texture2D.Dispose();
        }
    }
}