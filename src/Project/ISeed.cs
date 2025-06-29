namespace Shane32.SeedHelpers;

/// <summary>
/// Represents a seed operation for a specific type in a database.
/// </summary>
public interface ISeed<TDbContext, T> : ISeed<TDbContext>
{
}

/// <summary>
/// Represents a seed operation for a database.
/// </summary>
public interface ISeed<TDbContext>
{
    /// <summary>
    /// Seeds the database along with any dependencies.
    /// </summary>
    Task SeedAsync(TDbContext db, CancellationToken cancellationToken = default);
}
