namespace Analyze.Databases.LevelDb
{
    public enum RecordState
    {
        Delete = 0,
        Live,
        None
    }

    public enum BlockType : byte
    {
        Full = 1,
        First,
        Middle,
        Last
    }
}
