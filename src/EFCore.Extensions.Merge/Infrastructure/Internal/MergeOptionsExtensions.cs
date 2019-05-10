using System;
using EntityFrameworkCore.Extensions.Merge.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EntityFrameworkCore.Extensions.Merge.Infrastructure.Internal
{
	public class MergeOptionsExtensions : IDbContextOptionsExtension
	{
		public String LogFragment { get; } = "MergeExtensions";

		public Boolean ApplyServices(IServiceCollection services)
		{
			services.TryAddScoped<IMergeRawSqlGenerator, MergeRawSqlGenerator>();

			return true;
		}

		public Int64 GetServiceProviderHashCode() => 0L;
		public void Validate(IDbContextOptions options) { }
	}
}
