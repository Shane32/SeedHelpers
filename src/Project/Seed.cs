namespace Shane32.SeedHelpers;

/// <summary>
/// An abstract class that represents a seed operation for a specific type in a database.
/// </summary>
public abstract class Seed<TDbContext, T> : ISeed<TDbContext, T>
{
    /// <summary>
    /// Seeds the database for the specified type along with any dependencies.
    /// </summary>
    public abstract Task SeedAsync(TDbContext db, CancellationToken cancellationToken = default);
}
