using Application.Client.Models;
using Domain.Conventions;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.Extensions;
using System.Linq.Expressions;

namespace Application.Extensions;

public static class DbContextExtensions
{
	public static void MapDbSet<TEntity>(
		this IEndpointRouteBuilder routeBuilder, 
		string pattern, Func<ApplicationDbContext, DbSet<TEntity>> dbSet,
		Expression<Func<TEntity, bool>> filter) where TEntity : BaseTable
	{
		routeBuilder.MapGet(pattern, async (ApplicationDbContext db, HttpContext context) =>
		{
			db.CurrentUser = context.User.FromClaims<UserInfo>();
			return Results.Ok(await dbSet.Invoke(db).Where(filter).ToListAsync());
		});

		routeBuilder.MapGet($"{pattern}/{{id}}", async (ApplicationDbContext db, HttpContext context, int id) =>
		{
			db.CurrentUser = context.User.FromClaims<UserInfo>();
			var result = await dbSet.Invoke(db)
				.Where(row => filter && row.Id == id)
				.FirstOrDefaultAsync();

			if (result is null) return Results.NotFound();

			return Results.Ok(result);
		});

		routeBuilder.MapPost(pattern, async (ApplicationDbContext db, HttpContext context) =>
		{
			db.CurrentUser = context.User.FromClaims<UserInfo>();
			var widget = await context.Request.ReadFromJsonAsync<TEntity>() ?? throw new Exception("couldn't parse json");
			dbSet.Invoke(db).Save(widget);
			await db.SaveChangesAsync();
			return Results.Ok(widget);
		});

		routeBuilder.MapDelete($"{pattern}/{{id}}", async (ApplicationDbContext db, HttpContext context, int id) =>
		{
			db.CurrentUser = context.User.FromClaims<UserInfo>();
			await dbSet.Invoke(db).Where(row => filter.Invoke(db, row) && row.Id == id).ExecuteDeleteAsync();
			return Results.Ok();
		});
	}
}
