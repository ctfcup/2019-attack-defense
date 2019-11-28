namespace medlink
{
    public interface IAuthProvider
    {
        bool AddUserOrCheckPass(string login, string pass);
    }
}