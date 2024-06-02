using Domain;
using Domain.Conventions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Service;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public IUserInfo? CurrentUser { get; set; }

	public DbSet<Widget> Widgets { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUser).Assembly);
	}

	public override int SaveChanges()
	{
		AuditEntities();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		AuditEntities();
		return base.SaveChangesAsync(cancellationToken);
	}

	private void AuditEntities()
	{
		foreach (var entity in ChangeTracker.Entries<BaseTable>())
		{
			switch (entity.State)
			{
				case EntityState.Added:
					entity.Entity.CreatedBy = CurrentUser?.UserName;
					entity.Entity.DateCreated = LocalDateTime(CurrentUser?.TimeZoneId);
					break;
				case EntityState.Modified:
					entity.Entity.ModifiedBy = CurrentUser?.UserName;
					entity.Entity.DateModified = LocalDateTime(CurrentUser?.TimeZoneId);
					break;
			}
		}
	}

	private static DateTime LocalDateTime(string? timeZoneId)
	{
		var now = DateTime.UtcNow;
		if (string.IsNullOrWhiteSpace(timeZoneId)) return now;

		var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
		return TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
	}

}

public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private static IConfiguration Config => new ConfigurationBuilder()
		.AddJsonFile("appsettings.json", optional: false)
		.Build();

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var connectionString = Config.GetConnectionString("DefaultConnection");
		var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
		builder.UseSqlServer(connectionString);
		return new ApplicationDbContext(builder.Options);
	}
}