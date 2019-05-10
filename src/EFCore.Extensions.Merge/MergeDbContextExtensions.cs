using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Extensions.Merge;
using EntityFrameworkCore.Extensions.Merge.Internal;
using FastMember;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore
{
	public static class MergeDbContextExtensions
	{
		public static Task Merge<TEntity>(this DbContext db, TEntity[] data) where TEntity : class => db.Merge(data, new MergeOptions());

		public static Task Merge<TEntity>(this DbContext db, TEntity[] data, MergeType mergeType) where TEntity : class => db.Merge(data, new MergeOptions
		{
			MergeType = mergeType
		});

		public static async Task Merge<TEntity>(this DbContext db, TEntity[] data, MergeOptions options) where TEntity : class
		{
			var entityType = db.Model.FindEntityType(typeof(TEntity));

			var context = new MergeContext
			{
				Logger = db.GetService<ILoggerFactory>().CreateLogger("EntityFrameworkCore.Extensions.SqlServer.Merge"),
				MergeType = options.MergeType,
				Connection = db.Database.GetDbConnection() as SqlConnection,
				Transaction = db.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction,
				DestinationSchemaName = entityType.FindAnnotation(RelationalAnnotationNames.Schema).Value.ToString(),
				DestinationTableName = entityType.FindAnnotation(RelationalAnnotationNames.TableName).Value.ToString(),
				SearchOnProperties = entityType.FindPrimaryKey().Properties.ToList(),
				Properties = entityType.GetProperties().Where(x => !x.IsConcurrencyToken).ToList()
			};

			if (context.Connection.State == ConnectionState.Closed)
			{
				await context.Connection.OpenAsync();
			}

			var sw = Stopwatch.StartNew();
			var createTempTableCommandText = $"SELECT TOP 0 * INTO {context.TempTableName} FROM [{context.DestinationSchemaName}].[{context.DestinationTableName}]";
			using (var command = new SqlCommand(createTempTableCommandText, context.Connection))
			{
				await command.ExecuteNonQueryAsync();
			}

			context.Logger.LogInformation($"Executed DbCommand ({sw.ElapsedMilliseconds}ms){Environment.NewLine}{createTempTableCommandText}");

			sw.Restart();

			using (var bcp = new SqlBulkCopy(context.Connection, SqlBulkCopyOptions.Default, context.Transaction))
			using (var reader = ObjectReader.Create(data, context.Properties.Select(x => x.Name).ToArray()))
			{
				bcp.DestinationTableName = context.TempTableName;
				await bcp.WriteToServerAsync(reader);
			}

			context.Logger.LogInformation($"Executed SqlBulkCopy ({sw.ElapsedMilliseconds}ms) into {context.TempTableName}");
			sw.Restart();

			var generator = db.GetService<IMergeRawSqlGenerator>();
			//var generator = new MergeRawSqlGenerator();
			var commandText = generator.GenerateRawSql(context);

			using (var command = context.Connection.CreateCommand())
			{
				command.CommandText = commandText;
				await command.ExecuteNonQueryAsync();
			}

			context.Logger.LogInformation($"Executed DbCommand ({sw.ElapsedMilliseconds}ms){Environment.NewLine}{commandText}");
		}
	}
}
