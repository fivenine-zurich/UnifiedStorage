namespace UnifiedStorage
{
    public interface IPath
    {
        bool IsRoot { get; }

        string Combine(params string[] fragments);
    }
}