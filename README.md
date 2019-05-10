# EFCore Merge Extensions
An entity framework core extension that adds Sql Server Merge support.

## Setup
```csharp
services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=Application; Trusted_Connection=True; MultipleActiveResultSets=true");
    options.EnableMerge();
});
```

## Usage
```csharp
await context.Merge(...);
```

## Contributing
All contributions must be checked into their own branch, containing only work related to a single feature or bug. When the work is ready to be merged to main, and meets all definitions of done, you can create a pull request which will then be reviewed to make sure all unit tests are passing.

### Definition of Done
1. All code must be unit tested with passing unit tests
2. All code must meet the coding guidelines
3. All public interfaces and public POCO classes must be fully documented with meaningful documentation
