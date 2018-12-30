namespace Gwn.BlogEngine.Library.Interfaces
{
    public interface IBeLogger
    {
        void Log(string message, params string[] parameters);
    }
}
