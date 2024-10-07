using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Persistance
{
	public class RepositoryDbContext
	{
		private readonly IConfiguration _configuration;
		private readonly string _connectionString=string.Empty;

		public RepositoryDbContext(IConfiguration configuration)
		{
			_configuration = configuration;
			_connectionString = _configuration.GetConnectionString("SqlConnection");
		}

		public IDbConnection CreateConnection()
			=> new SqlConnection(_connectionString);
	}
}