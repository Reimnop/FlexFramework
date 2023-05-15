using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using FlexFramework.Core;
using FlexFramework.Logging;

namespace FlexFramework.Physics;

public class PhysicsWorld : IDisposable, IUpdateable
{
    public Simulation Simulation => simulation;
    public float TimeStep { get; set; } = 1.0f / 50.0f;
    public event Action? Step;

    private readonly FlexFrameworkMain engine;
    private readonly BufferPool bufferPool;
    private readonly ThreadDispatcher threadDispatcher;
    private readonly Simulation simulation;
    
    private readonly Dictionary<IShape, TypedIndex> shapeIndexMap = new Dictionary<IShape, TypedIndex>();

    private float t = 0.0f;
    
    public PhysicsWorld(FlexFrameworkMain engine)
    {
        this.engine = engine;
        
        bufferPool = new BufferPool();
        threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount - 2);
        
        simulation = Simulation.Create(bufferPool, 
            new NarrowPhaseCallbacks(new SpringSettings(30.0f, 1.0f)), 
            new PoseIntegratorCallbacks(new Vector3(0.0f, -9.81f, 0.0f), 0.1f, 0.1f), 
            new SolveDescription(8, 1));
    }

    public TypedIndex AddShape<T>(T shape) where T : unmanaged, IShape
    {
        if (shapeIndexMap.TryGetValue(shape, out TypedIndex index))
        {
            return index;
        }
        
        index = simulation.Shapes.Add(shape);
        shapeIndexMap.Add(shape, index);
        return index;
    }

    public void Update(UpdateArgs args)
    {
        t += args.DeltaTime;

        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        const int maxSteps = 4;
        
        int i = 0;
        while (t >= TimeStep && i < maxSteps)
        {
            simulation.Timestep(TimeStep, threadDispatcher);
            Step?.Invoke();
            t -= TimeStep;
            i++;
        }

        if (i == maxSteps)
        {
            engine.LogMessage(this, Severity.Warning, null, $"Physics simulation is running {t * 1000.0f:0.0}ms behind!");
            t = 0.0f;
        }
    }

    public void Dispose()
    {
        simulation.Dispose();
        threadDispatcher.Dispose();
        ((IDisposable) bufferPool).Dispose();
    }
}