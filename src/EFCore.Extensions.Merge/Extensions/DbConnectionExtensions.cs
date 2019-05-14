using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	internal static class DbConnectionExtensions
	{
		public static async Task EnsureOpenAsync(this DbConnection connection, CancellationToken token = default)
		{
			if (connection.State == ConnectionState.Broken)
			{
				connection.Close();
			}

			if (connection.State != ConnectionState.Open)
			{
				await connection.OpenAsync(token);
			}
		}
	}
}
