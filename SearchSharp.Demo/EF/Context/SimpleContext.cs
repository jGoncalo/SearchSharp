using Microsoft.EntityFrameworkCore;
using SearchSharp.Demo.EF.Tables;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SearchSharp.Demo.EF.Context;

public class SimpleContext : DbContext {
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    public SimpleContext() : base() {

    }
    public SimpleContext(DbContextOptions options) : base(options) {

    }

    public static SimpleContext MemoryContext(Action<SimpleContext>? populate = null){
        var opts = new DbContextOptionsBuilder<SimpleContext>()
        .UseInMemoryDatabase("InMemoryDatabase")
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

        if(populate != null){
            var popContext = new SimpleContext(opts);
            try{
                populate(popContext);
                popContext.SaveChanges();
            }
            catch {
                //Ignore
            }
        }
        
        return new SimpleContext(opts);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("InMemoryDatabase");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAddress>()
            .HasOne(addr => addr.User)
            .WithMany(usr => usr.Addresses)
            .HasForeignKey(addr => addr.UserId)
            .IsRequired(true);

        modelBuilder.Entity<UserAccount>()
            .HasIndex(usr => usr.Email)
            .IsUnique(true);

        base.OnModelCreating(modelBuilder);
    }
}