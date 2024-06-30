namespace UIHelper
{
    internal class UniqueIdGen : IUniqueIdGen
    {
        string IUniqueIdGen.Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
