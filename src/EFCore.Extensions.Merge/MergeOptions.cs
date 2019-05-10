using System;
using System.Collections.Generic;

namespace EntityFrameworkCore.Extensions.Merge
{
	public class MergeOptions
	{
		public MergeType MergeType { get; set; } = MergeType.InsertOrUpdateOrDelete;
		public IList<String> PropertiesToInclude = new List<String>();
		public IList<String> PropertiesToExclude = new List<String>();
		public IList<String> UpdateOnProperties = new List<String>();
	}
}
