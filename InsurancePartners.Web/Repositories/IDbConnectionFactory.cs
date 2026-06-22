using System.Data;

namespace InsurancePartners.Web.Repositories;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}