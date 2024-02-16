public class NodeInfo
{
    public int lineNum { get; private set; }
    public float bit { get; private set; }
    public float longBitNum { get; private set; }

    public NodeInfo(int lineNum, float bit, float longBitNum)
    {
        this.lineNum = lineNum;
        this.bit = bit;
        this.longBitNum = longBitNum;
    }
}
