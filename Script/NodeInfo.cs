public class NodeInfo
{
    public bool IsSkyNode { get; private set; }
    public int LineNum { get; private set; }
    public float Bit { get; private set; }
    public float LongBitNum { get; private set; }

    public NodeInfo(bool isSkyNode, int lineNum, float bit, float longBitNum)
    {
        this.IsSkyNode = isSkyNode;
        this.LineNum = lineNum;
        this.Bit = bit;
        this.LongBitNum = longBitNum;
    }
}
