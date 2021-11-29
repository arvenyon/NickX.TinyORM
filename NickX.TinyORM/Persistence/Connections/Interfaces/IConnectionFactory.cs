using NickX.TinyORM.Mapping.Interfaces;
using System.Data.SqlClient;

namespace NickX.TinyORM.Persistence.Connections.Interfaces
{
    public interface IConnectionFactory
    {
        public IMapping Mapping { get; set; }
        SqlConnection Create();
    }
}
