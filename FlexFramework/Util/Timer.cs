namespace FlexFramework.Util;

public class Timer
{
    private float time;
    private float delay;
    private Action action;
    
    public Timer(float delay, Action action)
    {
        this.delay = delay;
        this.action = action;
    }
    
    public void Update(float deltaTime)
    {
        time += deltaTime;
        if (time >= delay)
        {
            time -= delay;
            action();
        }
    }
}