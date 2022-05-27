namespace ApiTest20220523.Services
{
    public interface IJwtAuth
    {
        string Authentication(string username, string password);
    }
}