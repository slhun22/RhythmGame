public class NodeInfo
{
    public int lineNum { get; private set; }
    public float time { get; private set; }

    public NodeInfo(int lineNum, float time)
    {
        this.lineNum = lineNum;
        this.time = time;
    }
}
