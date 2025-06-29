
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

## Usage Examples

### Basic Seed Classes

Here are two sample seed classes where one depends on the other:

```cs
// Domain models
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

// Seed classes
public class UserSeed : Seed<TestDbContext, User>
{
    public override async Task SeedAsync(TestDbContext db, CancellationToken cancellationToken = default)
    {
        db.Users.AddRange(
            new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        );
    }
}

public class PostSeed : Seed<TestDbContext, Post>
{
    public override async Task SeedAsync(TestDbContext db, CancellationToken cancellationToken = default)
    {
        // First ensure users are seeded (dependency)
        await db.SeedAsync<User>(cancellationToken);
        
        db.Posts.AddRange(
            new Post
            {
                Id = 1,
                Title = "First Post",
                Content = "This is the first post content",
                UserId = 1
            },
            new Post
            {
                Id = 2,
                Title = "Second Post",
                Content = "This is the second post content",
                UserId = 2
            }
        );
    }
}
```

### Sample Test

Here's a sample test that seeds the `Post` entity, which automatically handles the `User` dependency:

```cs
[Fact]
public async Task Should_Create_Posts_With_Users()
{
    await Db.SeedAsync<Post>();
    
    var posts = await Db.Posts.Include(p => p.User).ToListAsync();
    posts.Count.ShouldBe(2);
    
    var users = await Db.Users.ToListAsync();
    users.Count.ShouldBe(2);
    
    posts.First().User.ShouldNotBeNull();
    posts.First().User.Name.ShouldBe("John Doe");
}
```

## Credits

Glory to Jehovah, Lord of Lords and King of Kings, creator of Heaven and Earth, who through his Son Jesus Christ,
has reedemed me to become a child of God. -Shane32
