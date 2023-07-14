using SharpFont;
using MsdfGenNet;
using FlexFramework.Text;
using GlyphMetrics = FlexFramework.Text.GlyphMetrics;

namespace FlexFramework.Text.Generator;

public class Program
{
    private static Library library = new();
    
    public static void Main(string[] args)
    {
        var fontPaths = new List<string>();
        Console.WriteLine("Scanning for fonts");
        foreach (var fontPath in Directory.EnumerateFiles("Fonts", "*.ttf", SearchOption.AllDirectories))
        {
            fontPaths.Add(fontPath);
        }
        Console.WriteLine($"Found {fontPaths.Count} fonts");
        
        var successCount = 0;
        var failureCount = 0;
        foreach (var fontPath in fontPaths)
        {
            Console.WriteLine($"Generating font: {fontPath}");
            try
            {
                var font = GenerateFont(fontPath);
                var path = Path.Combine("Generated Fonts", Path.GetFileNameWithoutExtension(fontPath) + ".flexfont");
                var directory = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directory);
                using var stream = File.OpenWrite(path);
                FontSerializier.Serialize(font, stream);
                successCount++;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to generate font: {fontPath}: {e}");
#if DEBUG
                throw;
#endif
                failureCount++;
            }
        }
        
        Console.WriteLine($"Generated {successCount} fonts (failed: {failureCount})");
    }

    private static Font GenerateFont(string fontPath)
    {
        const int size = 24;
        const int atlasWidth = 512;
        const string characterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{};':\",./<>?|\\`~©®™� ";

        using var face = new Face(library, fontPath);
        face.SetPixelSizes(0, size);
        
        // Get font metrics
        var height = face.Size.Metrics.Height.Value;
        var ascender = face.Size.Metrics.Ascender.Value;
        var descender = face.Size.Metrics.Descender.Value;
        var fontMetrics = new FontMetrics(size, height, ascender, descender);
        
        // Load glyphs
        var loadedGlyphs = new Dictionary<char, (Texture<Rgb32f>, GlyphMetrics)>();
        foreach (var c in characterSet)
        {
            LoadCharacter(face, c, out var texture, out var metrics);
            loadedGlyphs.Add(c, (texture, metrics));
        }
        
        // Load kernings
        // This is very slow, but it doesn't matter because we only need to do it once
        var kernings = new Dictionary<(char, char), int>();
        for (int i = 0; i < characterSet.Length; i++)
        {
            var c1 = characterSet[i];
            for (int j = 0; j < characterSet.Length; j++)
            {
                var c2 = characterSet[j];
                var g1 = face.GetCharIndex(c1);
                var g2 = face.GetCharIndex(c2);
                
                var kerning = face.GetKerning(g1, g2, KerningMode.Default);
                kernings.Add((c1, c2), kerning.X.Value);
            }
        }

        const int padding = 2;
        
        // Calculate atlas height
        int atlasHeight = CalculateAtlasHeight(loadedGlyphs.Values.Select(x => x.Item1), atlasWidth, padding);
        
        // Create atlas texture
        var glyphs = new Dictionary<char, GlyphInfo>();
        var atlasTexture = new Texture<Rgb32f>(atlasWidth, atlasHeight);
        var ptrX = 0;
        var ptrY = 0;
        var maxY = 0;
        foreach (var (c, (texture, metrics)) in loadedGlyphs)
        {
            // Check if we need to move to the next row
            if (ptrX + texture.Width > atlasTexture.Width)
            {
                ptrY += maxY + padding;
                maxY = 0;
                ptrX = 0;
            }
            
            // Copy texture to atlas
            atlasTexture.WritePartial(texture, ptrX, ptrY);

            // Calculate texture coordinates
            var minTexX = ptrX / (float) atlasWidth;
            var minTexY = ptrY / (float) atlasHeight;
            var maxTexX = (ptrX + texture.Width) / (float) atlasWidth;
            var maxTexY = (ptrY + texture.Height) / (float) atlasHeight;
            var texCoords = new TextureCoordinates(minTexX, minTexY, maxTexX, maxTexY);
            
            // Create glyph info
            glyphs[c] = new GlyphInfo(metrics, texCoords);
            
            // Advance ptrX and update maxY
            ptrX += texture.Width + padding;
            maxY = Math.Max(maxY, texture.Height);
        }
        
        // Create font
        var name = face.FamilyName!;
        return new Font(name, fontMetrics, atlasTexture, glyphs, kernings);
    }
    
    private static int CalculateAtlasHeight(IEnumerable<Texture<Rgb32f>> textures, int atlasWidth, int padding)
    {
        var ptrX = 0;
        var ptrY = 0;
        var maxY = 0;
        foreach (var texture in textures)
        {
            if (ptrX + texture.Width > atlasWidth)
            {
                ptrY += maxY + padding;
                maxY = 0; // Reset maxY
                ptrX = 0;
            }
            
            ptrX += texture.Width + padding;
            maxY = Math.Max(maxY, texture.Height);
        }

        return ptrY + maxY;
    }

    private static void LoadCharacter(Face face, char c, out Texture<Rgb32f> texture, out GlyphMetrics metrics)
    {
        const double range = 4.0;
        
        face.LoadChar(c, LoadFlags.Default, LoadTarget.Normal);
        var glyph = face.Glyph;
        var glyphMetrics = glyph.Metrics;
        
        // Use msdfgen to generate an msdf texture for the glyph
        var outline = glyph.Outline;
        var shapeBuilder = new ShapeBuilder(outline);
        var shape = shapeBuilder.Shape;
        shape.Normalize();
        
        // Set edge color
        MsdfGen.EdgeColoringByDistance(shape, 3.0);

        // Init output image
        var width = glyph.Metrics.Width.Value >> 6; // Equivalent to / 64
        var height = glyph.Metrics.Height.Value >> 6; // Equivalent to / 64
        using var output = new Bitmap<float>(width, height, 3);
        
        // Init error correction config
        var errorConfig = new ErrorCorrectionConfig();
        errorConfig.Mode = ErrorCorrectionMode.EdgePriority;
        errorConfig.DistanceCheckMode = DistanceCheckMode.CheckDistanceAtEdge;
        errorConfig.MinDeviationRatio = 0.0;
        errorConfig.MinImproveRatio = 0.0;
            
        // Init msdf config
        var config = new MSDFGeneratorConfig();
        config.OverlapSupport = true;
        config.ErrorCorrection = errorConfig;

        // Init projection
        var offsetX = glyphMetrics.HorizontalBearingX.Value / 64.0;
        var offsetY = (glyphMetrics.HorizontalBearingY.Value - glyphMetrics.Height.Value) / 64.0;
        
        var scale = new Vector2d(1.0f);
        var translate = new Vector2d(-offsetX, -offsetY);
        var projection = new Projection(ref scale, ref translate);

        // Generate msdf
        MsdfGen.GenerateMSDF(output, shape, projection, range, ref config);
        
        // Convert msdf to texture
        unsafe
        {
            fixed (float* ptr = output.Pixels)
            {
                var span = new ReadOnlySpan<Rgb32f>(ptr, output.Pixels.Length / 3);
                texture = new Texture<Rgb32f>(output.Width, output.Height, span);
            }
        }

        metrics = new GlyphMetrics(
            glyphMetrics.Width.Value,
            glyphMetrics.Height.Value,
            glyphMetrics.HorizontalAdvance.Value,
            glyphMetrics.VerticalAdvance.Value,
            glyphMetrics.HorizontalBearingX.Value,
            glyphMetrics.HorizontalBearingY.Value,
            glyphMetrics.VerticalBearingX.Value,
            glyphMetrics.VerticalBearingY.Value
        );
    }
}