namespace Tests;

public class SeedHandlerTests
{
    public class SampleDbContext
    {
        public bool Class1Seeded { get; set; }
        public bool Class2Seeded { get; set; }
    }

    public class Class1 { }

    public class Class2 { }

    public abstract class MySeed<T> : Seed<SampleDbContext, T>
    {
    }

    public class Class1Seed : MySeed<Class1>
    {
        public override Task SeedAsync(SampleDbContext db, CancellationToken cancellationToken)
        {
            db.Class1Seeded = true;
            return Task.CompletedTask;
        }
    }

    public class Class2Seed : MySeed<Class2>
    {
        public override Task SeedAsync(SampleDbContext db, CancellationToken cancellationToken)
        {
            db.Class2Seeded = true;
            return Task.CompletedTask;
        }
    }

    // Failing seed class that does not implement ISeed properly
    public class InvalidSeed { }

    [Fact]
    public async Task SeedAsync_Passing_ShouldSeedCorrectly()
    {
        // Arrange
        var dbContext = new SampleDbContext();
        var seedHandler = new SeedHandler<SampleDbContext>(dbContext);
        var seedType = typeof(Class1);

        // Act
        await seedHandler.SeedAsync(seedType);

        // Assert
        dbContext.Class1Seeded.ShouldBeTrue();
        dbContext.Class2Seeded.ShouldBeFalse();
    }

    [Fact]
    public async Task SeedAsync_Failing_WithInvalidType_ShouldNotSeed()
    {
        // Arrange
        var dbContext = new SampleDbContext();
        var seedHandler = new SeedHandler<SampleDbContext>(dbContext);
        var invalidType = typeof(InvalidSeed);

        // Act
        await seedHandler.SeedAsync(invalidType);

        // Assert
        dbContext.Class1Seeded.ShouldBeFalse();
        dbContext.Class2Seeded.ShouldBeFalse();
    }

    [Fact]
    public void GetSeedTypes_Passing_ShouldReturnAllSeedTypes()
    {
        // Arrange
        var expectedTypes = new[] { typeof(Class1), typeof(Class2) };

        // Act
        var seedTypes = SeedHandler<SampleDbContext>.GetSeedTypes().ToList();

        // Assert
        seedTypes.Sort((x, y) => x.Name.CompareTo(y.Name));
        seedTypes.ShouldBe(expectedTypes);
    }

    [Fact]
    public void GetSeedTypes_Failing_ShouldNotReturnNonSeedTypes()
    {
        // Arrange
        // Assuming InvalidSeed is not a seed type

        // Act
        var seedTypes = SeedHandler<SampleDbContext>.GetSeedTypes().ToList();

        // Assert
        seedTypes.ShouldNotContain(typeof(InvalidSeed));
    }

    [Fact]
    public async Task SeedAllAsync_Passing_ShouldSeedAllTypes()
    {
        // Arrange
        var dbContext = new SampleDbContext();
        var seedHandler = new SeedHandler<SampleDbContext>(dbContext);

        // Act
        await seedHandler.SeedAllAsync();

        // Assert
        dbContext.Class1Seeded.ShouldBeTrue();
        dbContext.Class2Seeded.ShouldBeTrue();
    }

    [Fact]
    public async Task Clear_Passing_ShouldClearSeededTypes()
    {
        // Arrange
        var dbContext = new SampleDbContext();
        var seedHandler = new SeedHandler<SampleDbContext>(dbContext);

        // Seed once
        await seedHandler.SeedAsync(typeof(Class1));
        dbContext.Class1Seeded.ShouldBeTrue();

        // Reset the seeded types by clearing
        seedHandler.Clear();

        // Reset the dbContext
        dbContext.Class1Seeded = false;

        // Act
        await seedHandler.SeedAsync(typeof(Class1));

        // Assert
        dbContext.Class1Seeded.ShouldBeTrue();
    }

    [Fact]
    public async Task Clear_Failing_ShouldNotAffectSeededTypesIfAlreadyCleared()
    {
        // Arrange
        var dbContext = new SampleDbContext();
        var seedHandler = new SeedHandler<SampleDbContext>(dbContext);

        // Seed once
        await seedHandler.SeedAsync(typeof(Class1));
        dbContext.Class1Seeded.ShouldBeTrue();

        // Clear seeded types
        seedHandler.Clear();

        // Clear again, should have no effect
        seedHandler.Clear();

        // Reset the dbContext
        dbContext.Class1Seeded = false;

        // Act
        await seedHandler.SeedAsync(typeof(Class1));

        // Assert
        dbContext.Class1Seeded.ShouldBeTrue();
    }
}
