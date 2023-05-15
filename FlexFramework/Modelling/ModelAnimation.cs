namespace FlexFramework.Modelling;

public class ModelAnimation
{
    public string Name { get; }
    public float DurationInTicks { get; }
    public float TicksPerSecond { get; }

    public IReadOnlyList<ModelNodeAnimationChannel> NodeAnimationChannels => nodeAnimationChannels;

    private readonly List<ModelNodeAnimationChannel> nodeAnimationChannels;
    private readonly Dictionary<string, int> nodeNameToChannelIndex;

    public ModelAnimation(string name, float durationInTicks, float ticksPerSecond, IEnumerable<ModelNodeAnimationChannel> nodeAnimationChannels)
    {
        Name = name;
        DurationInTicks = durationInTicks;
        TicksPerSecond = ticksPerSecond;
        this.nodeAnimationChannels = nodeAnimationChannels.ToList();
        nodeNameToChannelIndex = new Dictionary<string, int>();
        for (int i = 0; i < this.nodeAnimationChannels.Count; i++)
        {
            var channel = this.nodeAnimationChannels[i];
            nodeNameToChannelIndex[channel.NodeName] = i;
        }
    }

    public ModelNodeAnimationChannel? GetAnimationChannelFromName(string name)
    {
        if (nodeNameToChannelIndex.TryGetValue(name, out var index))
        {
            return nodeAnimationChannels[index];
        }

        return null;
    }
}