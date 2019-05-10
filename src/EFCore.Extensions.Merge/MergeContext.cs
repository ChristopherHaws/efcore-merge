using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Extensions.Merge
{
	public class MergeContext
	{
		public ILogger Logger { get; set; }
		public SqlConnection Connection { get; set; }
		public SqlTransaction Transaction { get; set; }
		public MergeType MergeType { get; set; }
		public String TempTableName => $"#{this.DestinationSchemaName}_{this.DestinationTableName}";
		public String DestinationSchemaName { get; set; }
		public String DestinationTableName { get; set; }
		public IList<IProperty> SearchOnProperties = new List<IProperty>();
		public IList<IProperty> Properties = new List<IProperty>();
	}
}
