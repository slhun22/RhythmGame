public class NodeInfo
{
    public int lineNum { get; private set; }
    public float bit { get; private set; }

    public NodeInfo(int lineNum, float bit)
    {
        this.lineNum = lineNum;
        this.bit = bit;
    }
}
