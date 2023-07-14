using HalfMaid.Async;

namespace FlexFramework.Core;

public class GameTaskManager : IUpdateable
{
    private GameTaskRunner runner = new();
    private float deltaTime;
    
    private HashSet<GameTask> tasks = new();

    public void Update(UpdateArgs args)
    {
        Update(args.DeltaTime);
    }

    public void Update(float deltaTime)
    {
        this.deltaTime = deltaTime;
        runner.RunNextFrame();

        // Check for exceptions
        foreach (var task in tasks)
            task.ExceptionDispatchInfo?.Throw();

        // Remove completed tasks
        tasks.RemoveWhere(x => x.IsCompleted);
    }
    
    public void StartImmediately(Func<GameTask> task)
    {
        runner.StartImmediately(() =>
        {
            var t = task();
            tasks.Add(t);
            return t;
        });
    }
    
    public void StartYielded(Func<GameTask> task)
    {
        runner.StartYielded(() =>
        {
            var t = task();
            tasks.Add(t);
            return t;
        });
    }

    public GameTaskYieldAwaitable WaitUntilNextFrame() => runner.Next();
    public GameTaskYieldAwaitable DelayFrames(int frames) => runner.Delay(frames);
    public ExternalTaskAwaitable RunTask(Func<Task> task) => runner.RunTask(task);

    public async GameTask WaitSeconds(float seconds)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            await WaitUntilNextFrame();
        }
    }
    
    public async GameTask RunForSeconds(float seconds, Action<float> task)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            task(t);
            await WaitUntilNextFrame(); // Prevents infinite loop
        }
    }
    
    public async GameTask RunForSecondsNormalized(float seconds, Action<float> task)
    {
        float t = 0;
        while (t < seconds)
        {
            t += deltaTime;
            task(t / seconds);
            await WaitUntilNextFrame(); // Prevents infinite loop
        }
    }
}