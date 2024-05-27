using Domain.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Service.Extensions;

public static class DbSetExtensions
{
    public static void Save<TEntity>(this DbSet<TEntity> entities, TEntity entity) where TEntity : BaseTable
    {
        if (entity.Id == default)
        {
            entities.Add(entity);
        }
        else
        {
            entities.Update(entity);
        }
    }
}
