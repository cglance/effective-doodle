using System.Threading.Tasks;

namespace PriceManagement.Common.Database
{
    public interface IDatasource
    {
        Task<IDatasourceConnection> GetConnectionAsync();
    }
}