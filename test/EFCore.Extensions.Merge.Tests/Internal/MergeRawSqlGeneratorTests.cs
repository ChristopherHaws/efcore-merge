using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using NSubstitute;
using Xunit;

namespace EntityFrameworkCore.Extensions.Merge.Internal.Tests
{
	public class MergeRawSqlGeneratorTests
	{
		[Fact]
		public void GenerateRawSql_Insert()
		{
			// Arrange
			var pk_1 = Property("pk_1");
			var pk_2 = Property("pk_2");
			var property1 = Property("property1");
			var property2 = Property("property2");

			var context = new MergeContext
			{
				MergeType = MergeType.Insert,
				DestinationSchemaName = "dbo",
				DestinationTableName = "DestinationTable",
				SearchOnProperties = { pk_1, pk_2 },
				Properties = { pk_1, pk_2, property1, property2 }
			};

			var generator = new MergeRawSqlGenerator();

			// Act
			var sql = generator.GenerateRawSql(context);

			// Assert
			sql.Should().Be(@"
MERGE INTO [dbo].[DestinationTable] AS target
USING #dbo_DestinationTable AS source ON source.[pk_1] = target.[pk_1] AND source.[pk_2] = target.[pk_2]
WHEN NOT MATCHED BY TARGET THEN INSERT ([pk_1], [pk_2], [property1], [property2]) VALUES (source.[pk_1], source.[pk_2], source.[property1], source.[property2]);".Trim());
		}

		[Fact]
		public void GenerateRawSql_InsertOrUpdate()
		{
			// Arrange
			var pk_1 = Property("pk_1");
			var pk_2 = Property("pk_2");
			var property1 = Property("property1");
			var property2 = Property("property2");

			var context = new MergeContext
			{
				MergeType = MergeType.InsertOrUpdate,
				DestinationSchemaName = "dbo",
				DestinationTableName = "DestinationTable",
				SearchOnProperties = { pk_1, pk_2 },
				Properties = { pk_1, pk_2, property1, property2 }
			};

			var generator = new MergeRawSqlGenerator();

			// Act
			var sql = generator.GenerateRawSql(context);

			// Assert
			sql.Should().Be(@"
MERGE INTO [dbo].[DestinationTable] AS target
USING #dbo_DestinationTable AS source ON source.[pk_1] = target.[pk_1] AND source.[pk_2] = target.[pk_2]
WHEN MATCHED THEN UPDATE SET target.[pk_1] = source.[pk_1], target.[pk_2] = source.[pk_2], target.[property1] = source.[property1], target.[property2] = source.[property2]
WHEN NOT MATCHED BY TARGET THEN INSERT ([pk_1], [pk_2], [property1], [property2]) VALUES (source.[pk_1], source.[pk_2], source.[property1], source.[property2]);".Trim());
		}

		[Fact]
		public void GenerateRawSql_InsertOrUpdateOrDelete()
		{
			// Arrange
			var pk_1 = Property("pk_1");
			var pk_2 = Property("pk_2");
			var property1 = Property("property1");
			var property2 = Property("property2");

			var context = new MergeContext
			{
				MergeType = MergeType.InsertOrUpdateOrDelete,
				DestinationSchemaName = "dbo",
				DestinationTableName = "DestinationTable",
				SearchOnProperties = { pk_1, pk_2 },
				Properties = { pk_1, pk_2, property1, property2 }
			};

			var generator = new MergeRawSqlGenerator();

			// Act
			var sql = generator.GenerateRawSql(context);

			// Assert
			sql.Should().Be(@"
MERGE INTO [dbo].[DestinationTable] AS target
USING #dbo_DestinationTable AS source ON source.[pk_1] = target.[pk_1] AND source.[pk_2] = target.[pk_2]
WHEN MATCHED THEN UPDATE SET target.[pk_1] = source.[pk_1], target.[pk_2] = source.[pk_2], target.[property1] = source.[property1], target.[property2] = source.[property2]
WHEN NOT MATCHED BY TARGET THEN INSERT ([pk_1], [pk_2], [property1], [property2]) VALUES (source.[pk_1], source.[pk_2], source.[property1], source.[property2])
WHEN NOT MATCHED BY SOURCE THEN DELETE;".Trim());
		}

		private static IProperty Property(String name)
		{
			var property = Substitute.For<IProperty>();
			property.Name.Returns(name);

			return property;
		}
	}
}
