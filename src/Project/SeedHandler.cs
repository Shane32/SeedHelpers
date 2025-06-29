namespace Shane32.SeedHelpers;

/// <summary>
/// Handles seeding of a test database.
/// </summary>
public class SeedHandler<TDbContext>
{
    private static readonly ILookup<Type, Func<TDbContext, CancellationToken, Task>> _seedDelegates;
    private readonly HashSet<Type> _seeded = [];
    private readonly TDbContext _db;

    static SeedHandler()
    {
        // generate a lookup table by data model for all ISeed<T>s that match that model, returning a list of delegates for the T
        _seedDelegates = (
        // find all classes that implement ISeed<TDbContext>
            from classType in typeof(TDbContext).Assembly.GetTypes()
            where classType.IsClass && (!classType.IsGenericType || classType.IsConstructedGenericType) && typeof(ISeed<TDbContext>).IsAssignableFrom(classType)
            // then pull the list of ISeed<TDbContext, T> interfaces that they implement
            select new {
                ClassType = classType,
                ISeedInterfaces = classType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ISeed<,>))
            })
            // then select the ClrType and ClassType for each ISeed<T> interface
            .SelectMany(x => x.ISeedInterfaces.Select(y => new {
                ClrType = y.GetGenericArguments()[1], // the T in ISeed<TDbContext, T>
                x.ClassType,                          // the class that implements ISeed<TDbContext, T>
            }))
            // then create a delegate based on the ClassType
            .Select(x => new {
                x.ClrType,
                Func = CreateSeedDelegate(x.ClassType),
            })
            // then convert to a lookup table
            .ToLookup(x => x.ClrType, x => x.Func);
        Func<TDbContext, CancellationToken, Task> CreateSeedDelegate(Type type)
            => (testDb, cancellationToken) => ((ISeed<TDbContext>)Activator.CreateInstance(type)!).SeedAsync(testDb, cancellationToken);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedHandler{TDbContext}"/> class.
    /// </summary>
    public SeedHandler(TDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Seeds the database with the specified type and any dependencies.
    /// </summary>
    public async Task SeedAsync(Type type, CancellationToken cancellationToken = default)
    {
        if (!_seeded.Add(type))
            return;
        foreach (var mapping in _seedDelegates[type]) {
            await mapping(_db, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the seed types.
    /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static IEnumerable<Type> GetSeedTypes()
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        return _seedDelegates.Select(x => x.Key).Distinct();
    }

    /// <summary>
    /// Seeds the database with all types.
    /// </summary>
    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (var type in GetSeedTypes()) {
            await SeedAsync(type, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Clears the list of seeded types.
    /// </summary>
    public void Clear()
    {
        _seeded.Clear();
    }
}
