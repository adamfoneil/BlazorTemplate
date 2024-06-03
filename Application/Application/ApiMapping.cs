using Application.Abstract;
using Application.Client.Models;
using Domain.Conventions;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Service;
using Service.Extensions;

namespace Application;

internal class ApiMapping<TEntity>(string pattern) : DbSetApiMapping<ApplicationDbContext, TEntity, int>(pattern) where TEntity : BaseTable
{
	protected override async Task DeleteAsync(ApplicationDbContext dbContext, HttpContext httpContext, int id)
	{
		dbContext.CurrentUser = httpContext.User.FromClaims<UserInfo>();

		await dbContext.Set<TEntity>()
			.Where(row => row.CreatedBy == dbContext.CurrentUser.UserName && row.Id == id)
			.ExecuteDeleteAsync();
	}

	protected override async Task<IEnumerable<TEntity>> GetAllAsync(ApplicationDbContext dbContext, HttpContext httpContext)
	{
		dbContext.CurrentUser = httpContext.User.FromClaims<UserInfo>();

		return await dbContext.Set<TEntity>()
			.Where(row => row.CreatedBy == dbContext.CurrentUser.UserName)
			.ToListAsync();
	}

	protected override async Task<TEntity?> GetAsync(ApplicationDbContext dbContext, HttpContext httpContext, int id)
	{
		dbContext.CurrentUser = httpContext.User.FromClaims<UserInfo>();

		return await dbContext.Set<TEntity>()
			.Where(row => row.CreatedBy == dbContext.CurrentUser.UserName && row.Id == id)
			.FirstOrDefaultAsync();
	}

	protected override void InsertOrUpdate(ApplicationDbContext dbContext, HttpContext httpContext, TEntity entity)
	{
		dbContext.CurrentUser = httpContext.User.FromClaims<UserInfo>();
		dbContext.Set<TEntity>().Save(entity);
	}
}