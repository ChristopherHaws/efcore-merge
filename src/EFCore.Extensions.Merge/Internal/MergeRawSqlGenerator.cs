using System;
using System.Linq;
using System.Text;

namespace EntityFrameworkCore.Extensions.Merge.Internal
{
	public interface IMergeRawSqlGenerator
	{
		String GenerateRawSql(MergeContext context);
	}

	public class MergeRawSqlGenerator : IMergeRawSqlGenerator
	{
		public String GenerateRawSql(MergeContext context)
		{
			var sb = new StringBuilder();

			var searchOnPropertyNames = String.Join(" AND ", context.SearchOnProperties.Select(x => $"source.[{x.Name}] = target.[{x.Name}]").ToArray());

			var updatePropertyNames = String.Join(", ", context.Properties.Select(x => $"target.[{x.Name}] = source.[{x.Name}]").ToArray());
			var insertPropertyNames = String.Join(", ", context.Properties.Select(x => $"[{x.Name}]").ToArray());
			var insertPropertyValues = String.Join(", ", context.Properties.Select(x => $"source.[{x.Name}]").ToArray());

			sb.AppendLine($"MERGE INTO [{context.DestinationSchemaName}].[{context.DestinationTableName}] AS target");
			sb.AppendLine($"USING {context.TempTableName} AS source ON {searchOnPropertyNames}");

			if (context.MergeType == MergeType.InsertOrUpdate || context.MergeType == MergeType.InsertOrUpdateOrDelete)
			{
				sb.AppendLine($"WHEN MATCHED THEN UPDATE SET {updatePropertyNames}");
			}

			if (context.MergeType == MergeType.Insert || context.MergeType == MergeType.InsertOrUpdate || context.MergeType == MergeType.InsertOrUpdateOrDelete)
			{
				sb.AppendLine($"WHEN NOT MATCHED BY TARGET THEN INSERT ({insertPropertyNames}) VALUES ({insertPropertyValues})");
			}

			if (context.MergeType == MergeType.InsertOrUpdateOrDelete)
			{
				sb.AppendLine($"WHEN NOT MATCHED BY SOURCE THEN DELETE");
			}

			return String.Concat(sb.ToString().TrimEnd(), ";");
		}
	}
}
