namespace Defong.ObjectPool
{
    public interface IPrefable
    {
        string PrefabID { get; }
        string PrefabPrefix { get; }
        int? GlobalIndex { get; set; }
    }
}