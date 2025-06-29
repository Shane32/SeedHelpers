
# Shane32.SeedHelpers

[![NuGet](https://img.shields.io/nuget/v/Shane32.SeedHelpers.svg)](https://www.nuget.org/packages/Shane32.SeedHelpers)

This library contains a set of helper classes and methods to facilitate the seeding databases within tests.

Sample use with EF Core database:

```cs
public class TestDbContext : AppDbContext
{
    private readonly SeedHandler<TestDbContext> _seedHandler;

    public TestDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _seedHandler = new(this);
    }

    public Task SeedAsync<T>(CancellationToken cancellationToken = default)
    {
        await _seedHandler.SeedAsync(typeof(T), cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        await _seedHandler.SeedAllAsync(cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public static IEnumerable<Type> GetSeedTypes()
    {
        return SeedHandler<TestDbContext>.GetSeedTypes();
    }
}
```

## Credits

Glory to Jehovah, Lord of Lords and King of Kings, creator of Heaven and Earth, who through his Son Jesus Christ,
has reedemed me to become a child of God. -Shane32
