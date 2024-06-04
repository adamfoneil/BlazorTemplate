using Microsoft.EntityFrameworkCore;

namespace BackendHelpers;

public abstract class DbSetApiMapping<TDbContext, TEntity, TKey>(string pattern)
	where TDbContext : DbContext
	where TEntity : class
	where TKey : struct
{
	private readonly string _pattern = pattern;

	protected abstract Task<IEnumerable<TEntity>> GetAllAsync(TDbContext dbContext, HttpContext httpContext);

	protected abstract Task<TEntity?> GetAsync(TDbContext dbContext, HttpContext httpContext, TKey id);

	protected abstract Task DeleteAsync(TDbContext dbContext, HttpContext httpContext, TKey id);

	protected abstract void InsertOrUpdate(TDbContext dbContext, HttpContext httpContext, TEntity entity);

	public void Map(IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapGet(_pattern, async (TDbContext db, HttpContext context) =>
		{
			var results = await GetAllAsync(db, context);
			return Results.Ok(results);
		});

		routeBuilder.MapGet($"{_pattern}/{{id}}", async (TDbContext db, HttpContext context, TKey id) =>
		{
			var result = await GetAsync(db, context, id);
			if (result is null) return Results.NotFound();
			return Results.Ok(result);
		});

		routeBuilder.MapPost(_pattern, async (TDbContext db, HttpContext context) =>
		{
			var entity = await context.Request.ReadFromJsonAsync<TEntity>() ?? throw new Exception($"couldn't parse json as type {typeof(TEntity).Name}");
			InsertOrUpdate(db, context, entity);
			await db.SaveChangesAsync();
			return Results.Ok(entity);
		});

		routeBuilder.MapDelete($"{_pattern}/{{id}}", async (TDbContext db, HttpContext context, TKey id) =>
		{
			await DeleteAsync(db, context, id);
			return Results.Ok();
		});
	}
}
