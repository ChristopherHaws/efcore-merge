namespace EntityFrameworkCore.Extensions.Merge
{
	public enum MergeType
	{
		/// <summary>
		/// Inserts the values that don't already exist in the destination table.
		/// </summary>
		Insert,
		InsertOrUpdate,
		InsertOrUpdateOrDelete
	}
}
