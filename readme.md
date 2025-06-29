
# Shane32.SeedHelpers

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
