public class NodeInfo
{
    public bool isSkyNode { get; private set; }
    public int lineNum { get; private set; }
    public float bit { get; private set; }
    public float longBitNum { get; private set; }

    public NodeInfo(bool isSkyNode, int lineNum, float bit, float longBitNum)
    {
        this.isSkyNode = isSkyNode;
        this.lineNum = lineNum;
        this.bit = bit;
        this.longBitNum = longBitNum;
    }
}
