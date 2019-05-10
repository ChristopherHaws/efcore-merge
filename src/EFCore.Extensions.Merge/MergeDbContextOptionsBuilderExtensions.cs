using EntityFrameworkCore.Extensions.Merge.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
	public static class MergeDbContextOptionsBuilderExtensions
	{
		public static DbContextOptionsBuilder EnableMerge(this DbContextOptionsBuilder optionsBuilder)
		{
			var extension = optionsBuilder.Options.FindExtension<MergeOptionsExtensions>() ?? new MergeOptionsExtensions();
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			return optionsBuilder;
		}
	}
}
