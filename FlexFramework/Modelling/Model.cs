using FlexFramework.Core.Data;
using OpenTK.Mathematics;

namespace FlexFramework.Modelling;

public class ModelMaterial
{
    public string Name { get; }
    public Vector3 Albedo { get; set; }
    public float Metallic { get; set; } 
    public float Roughness { get; set; } 
    public Texture? AlbedoTexture { get; set; }
    public Texture? MetallicTexture { get; set; }
    public Texture? RoughnessTexture { get; set; }

    public ModelMaterial(
        string name, 
        Vector3 albedo, float metallic, float roughness, 
        Texture? albedoTexture, Texture? metallicTexture, Texture? roughnessTexture)
    {
        Name = name;
        Albedo = albedo;
        Metallic = metallic;
        Roughness = roughness;
        AlbedoTexture = albedoTexture;
        MetallicTexture = metallicTexture;
        RoughnessTexture = roughnessTexture;
    }
}

public class ModelMesh
{
    public int MeshIndex { get; }
    public int MaterialIndex { get; }

    public ModelMesh(int meshIndex, int materialIndex)
    {
        MeshIndex = meshIndex;
        MaterialIndex = materialIndex;
    }
}

public class ModelNode
{
    public string Name { get; }
    public Matrix4 Transform { get; }
    public IReadOnlyList<ModelMesh> Meshes => meshes;

    private readonly List<ModelMesh> meshes;

    public ModelNode(string name, Matrix4 transform, List<ModelMesh> meshes)
    {
        Name = name;
        Transform = transform;
        this.meshes = meshes;
    }
}

public class ModelBone
{
    public string Name { get; }
    public int Index { get; }
    public Matrix4 Offset { get; }

    public ModelBone(string name, int index, Matrix4 offset)
    {
        Name = name;
        Index = index;
        Offset = offset;
    }
}

public class Model : IDisposable
{
    private readonly ModelImporter modelImporter;
    
    public ImmutableNode<ModelNode> RootNode { get; }
    public IReadOnlyList<ModelBone> Bones { get; }
    public IReadOnlyDictionary<string, int> BoneIndexMap { get; }
    
    // Lazily load everything
    public IReadOnlyList<Mesh<LitVertex>> Meshes => meshes ??= lazyMeshes.ToList();
    public IReadOnlyList<Mesh<SkinnedVertex>> SkinnedMeshes => skinnedMeshes ??= lazySkinnedMeshes.ToList();
    public IReadOnlyList<ModelMaterial> Materials => materials ??= lazyMaterials.ToList();
    public IReadOnlyList<ModelAnimation> Animations => animations ??= lazyAnimations.ToList();
    
    // Internal collections
    private List<Mesh<LitVertex>>? meshes;
    private List<Mesh<SkinnedVertex>>? skinnedMeshes;
    private List<ModelMaterial>? materials;
    private List<ModelAnimation>? animations;

    private readonly IEnumerable<Mesh<LitVertex>> lazyMeshes;
    private readonly IEnumerable<Mesh<SkinnedVertex>> lazySkinnedMeshes;
    private readonly IEnumerable<ModelMaterial> lazyMaterials;
    private readonly IEnumerable<ModelAnimation> lazyAnimations;

    public Model(string path)
    {
        modelImporter = new ModelImporter(path);
        
        RootNode = modelImporter.LoadModel();
        Bones = modelImporter.Bones;
        BoneIndexMap = modelImporter.BoneIndexMap;
        
        // This doesn't actually "load" anything yet
        lazyMeshes = modelImporter.LoadMeshes();
        lazySkinnedMeshes = modelImporter.LoadSkinnedMeshes();
        lazyMaterials = modelImporter.LoadMaterials();
        lazyAnimations = modelImporter.LoadAnimations();
    }

    public void Dispose()
    {
        modelImporter.Dispose();
    }
}