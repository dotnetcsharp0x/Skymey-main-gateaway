
namespace Skymey_main_gateaway.Data
{
    public class MongoDbSettings : IAsyncDisposable
    {
        public string Server { get; set; }
        public string Port { get; set; }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync() => default;

        ~MongoDbSettings()
        {

        }
    }
}
