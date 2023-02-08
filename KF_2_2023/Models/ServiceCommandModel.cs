using KF_2_2023.DataAccess;
using Redis.OM.Modeling;

namespace KF_2_2023.Models
{
    [Document]
    public class ServiceCommandModel
    {
        [Indexed]
        public string id { get; set; } = string.Empty;
        public long timestamp { get; set; } = long.MinValue;
        public string command { get; set; } = string.Empty;
        public string commander { get; set; } = string.Empty;
        public bool success { get; set; } = false;

        public ServiceCommandModel(string _comand, string _commander)
        {
            id = Guid.NewGuid().ToString();
            timestamp = DateTime.Now.Ticks;
            command = _commander;
            commander = _commander;
        }
    }
}
