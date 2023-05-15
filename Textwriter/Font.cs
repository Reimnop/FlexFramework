using System.Numerics;
using HardFuzz.HarfBuzz;
using MsdfGenNet;
using SharpFont;
using Buffer = HardFuzz.HarfBuzz.Buffer;
using FtFace = SharpFont.Face;
using HbFont = HardFuzz.HarfBuzz.Font;
using HbGlyphInfo = HardFuzz.HarfBuzz.GlyphInfo;
using FtGlyph = SharpFont.Glyph;

namespace Textwriter;

public class Font : IDisposable
{
    public string FamilyName { get; }
    public int Size { get; }
    public int Height { get; }
    public int Ascender { get; }
    public int Descender { get; }
    public int TotalHeight => Ascender - Descender;

    public AtlasTexture<Vector3> Atlas { get; }

    private readonly FtFace ftFace;
    private readonly HbFont hbFont;

    private readonly Glyph[] glyphs;

    public Font(Library library, string path, int size, int atlasWidth)
    {
        const double range = 4.0;
        const double textureScale = 1.0;
        
        Size = size;
        
        ftFace = new FtFace(library, path);
        ftFace.SetPixelSizes(0, (uint) size);

        FamilyName = ftFace.FamilyName;
        Height = ftFace.Size.Metrics.Height.Value;
        Ascender = ftFace.Size.Metrics.Ascender.Value;
        Descender = ftFace.Size.Metrics.Descender.Value;

        List<ClientTexture<Vector3>> glyphTextures = new List<ClientTexture<Vector3>>();

        glyphs = new Glyph[ftFace.GlyphCount];
        for (uint i = 0; i < glyphs.Length; i++)
        {
            ftFace.LoadGlyph(i, LoadFlags.Default, LoadTarget.Normal);
            
            GlyphSlot glyph = ftFace.Glyph;
            GlyphMetrics metrics = glyph.Metrics;
            
            // Get glyph size
            int width = (int) (metrics.Width.Value / 64.0 * textureScale);
            int height = (int) (metrics.Height.Value / 64.0 * textureScale);

            double offsetX = metrics.HorizontalBearingX.Value / 64.0;
            double offsetY = (metrics.HorizontalBearingY.Value - metrics.Height.Value) / 64.0;

            // Use msdfgen to generate an msdf texture for the glyph
            Outline outline = glyph.Outline;
            ShapeBuilder shapeBuilder = new ShapeBuilder(outline);
            Shape shape = shapeBuilder.Shape;
            shape.Normalize();

            MsdfGen.EdgeColoringByDistance(shape, 3.0);

            // Init output image
            using Bitmap<float> output = new Bitmap<float>(width, height, 3);
            
            ErrorCorrectionConfig errorConfig = new ErrorCorrectionConfig();
            errorConfig.Mode = ErrorCorrectionMode.EdgePriority;
            errorConfig.DistanceCheckMode = DistanceCheckMode.CheckDistanceAtEdge;
            errorConfig.MinDeviationRatio = 0.0;
            errorConfig.MinImproveRatio = 0.0;
            
            MSDFGeneratorConfig config = new MSDFGeneratorConfig();
            config.OverlapSupport = true;
            config.ErrorCorrection = errorConfig;

            Vector2d scale = new Vector2d(textureScale);
            Vector2d translate = new Vector2d(-offsetX, -offsetY);
            Projection projection = new Projection(ref scale, ref translate);

            // Generate msdf
            MsdfGen.GenerateMSDF(output, shape, projection, range, ref config);

            // Copy bitmap to client texture
            ClientTexture<Vector3> glyphTexture = new ClientTexture<Vector3>(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Span<float> pixel = output[x, height - 1 - y];
                    glyphTexture[x, y] = new Vector3(pixel[0], pixel[1], pixel[2]);
                }
            }
            
            // Add glyph texture to atlas
            glyphTextures.Add(glyphTexture);
            
            // Create glyph
            glyphs[i].Width = metrics.Width.Value;
            glyphs[i].Height = metrics.Height.Value;
            glyphs[i].AdvanceX = metrics.HorizontalAdvance.Value;
            glyphs[i].AdvanceY = metrics.VerticalAdvance.Value;
            glyphs[i].HorizontalBearingX = metrics.HorizontalBearingX.Value;
            glyphs[i].HorizontalBearingY = metrics.HorizontalBearingY.Value;
            glyphs[i].VerticalBearingX = metrics.VerticalBearingX.Value;
            glyphs[i].VerticalBearingY = metrics.VerticalBearingY.Value;
        }
        
        Atlas = new AtlasTexture<Vector3>(atlasWidth, CalculateAtlasHeight(glyphTextures, atlasWidth));
        for (int i = 0; i < glyphs.Length; i++)
        {
            glyphs[i].Uv = Atlas.AddGlyphTexture(glyphTextures[i]);
        }
        
        hbFont = HbFont.FromFreeType(ftFace.Reference);
    }

    private int CalculateAtlasHeight(IEnumerable<IClientTexture> textures, int atlasWidth)
    {
        int ptrX = 0;
        int ptrY = 0;
        int maxY = 0;
        foreach (IClientTexture texture in textures)
        {
            if (ptrX + texture.Width > atlasWidth)
            {
                ptrY += maxY + 4;
                maxY = 0;
                ptrX = 0;
            }
            
            ptrX += texture.Width + 4;
            maxY = Math.Max(maxY, texture.Height);
        }

        return ptrY + maxY;
    }

    public IEnumerable<GlyphInfo> ShapeText(string text)
    {
        using Buffer buffer = new Buffer();
        buffer.AddUtf(text);
        buffer.GuessSegmentProperties();
        buffer.Shape(hbFont);

        GlyphPosition[] glyphPositions = buffer.GlyphPositions.ToArray();
        HbGlyphInfo[] glyphInfos = buffer.GlyphInfos.ToArray();
        
        for (int i = 0; i < buffer.Length; i++)
        {
            yield return new GlyphInfo(this, 
                glyphPositions[i].XAdvance, glyphPositions[i].YAdvance, 
                glyphPositions[i].XOffset, glyphPositions[i].YOffset, 
                (int) glyphInfos[i].Codepoint);
        }
    }

    public Glyph GetGlyph(int index)
    {
        return glyphs[index];
    }

    public void Dispose()
    {
        hbFont.Dispose();
        ftFace.Dispose();
    }
}