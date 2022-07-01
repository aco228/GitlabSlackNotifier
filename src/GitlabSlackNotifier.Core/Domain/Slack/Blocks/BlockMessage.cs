using GitlabSlackNotifier.Core.Domain.Slack.Blocks.Core;

namespace GitlabSlackNotifier.Core.Domain.Slack.Blocks;

public class BlockMessage
{
    public List<BlockBase> Blocks { get; set; } = new();

    public void Add(BlockBase block)
        => Blocks.Add(block);
}