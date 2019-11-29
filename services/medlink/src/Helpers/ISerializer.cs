namespace medlink.Helpers
{
    public interface ISerializer
    {
        string Serialize<T>(T source);
        T Deserialize<T>(string source);
    }
}