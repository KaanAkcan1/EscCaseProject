using EscCase.Common.Core;
using EscCase.Common.Entities.Common;
using EscCase.Common.Enums.Common;
using EscCase.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EscCase.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;


        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table => _context.Set<T>();

        #region WriteRepository
        public async Task<RepositoryResult<T>> CreateAsync(T model)
        {
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = model.CreatedBy == Guid.Empty ? RepositoryDefaults.UserId.System : model.CreatedBy;
            model.Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id;
            model.ModifiedOn = DateTime.Now;
            model.ModifiedBy = model.ModifiedBy == Guid.Empty ? RepositoryDefaults.UserId.System : model.ModifiedBy;
            model.StatusId = (int)EntityStatus.Active;

            EntityEntry<T> entityEntry = await Table.AddAsync(model);

            var data = new List<T>();

            if (entityEntry.State == EntityState.Added)
                data.Add(model);

            var result = new RepositoryResult<T>()
            {
                Success = entityEntry.State == EntityState.Added ? true : false,
                Message = entityEntry.State == EntityState.Added ? "Ok" : "Hata Alındı",
                Data = data
            };

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
            }

            return result;
        }

        public async Task<RepositoryResult<T>> CreateAsync(List<T> datas)
        {
            var data = new List<T>();

            var result = new RepositoryResult<T>()
            {
                Success = true,
                Message = "Ok",
                Data = data
            };

            foreach (var model in datas)
            {

                var entityExists = await Table.AnyAsync(x => x.Id == model.Id);

                if (entityExists)
                {
                    result.Success = false;
                    result.Message = $"Entity with Id {model.Id} already exists.";
                    data.Add(model);
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    model.CreatedBy = model.CreatedBy == Guid.Empty ? new Guid("99999999-9999-9999-9999-999999999999") : model.CreatedBy;
                    model.ModifiedBy = model.ModifiedBy == Guid.Empty ? new Guid("99999999-9999-9999-9999-999999999999") : model.ModifiedBy;
                    model.ModifiedOn = DateTime.Now;
                    model.StatusId = (int)EntityStatus.Active;

                    EntityEntry<T> entityEntry = await Table.AddAsync(model);

                    if (entityEntry.State != EntityState.Added)
                    {
                        result.Success = false;
                        data.Add(model);
                    }

                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;

                result.Message = ex.Message;
            }


            return result;
        }

        public async Task<RepositoryResult<T>> CreateUpdateAsync(T model)
        {
            var data = new List<T>();

            var result = new RepositoryResult<T>()
            {
                Success = true,
                Message = "Ok",
                Data = data
            };

            if (await Table.AnyAsync(x => x.Id == model.Id))
            {
                try
                {
                    model.ModifiedOn = DateTime.Now;
                    model.ModifiedBy = model.ModifiedBy == Guid.Empty ? RepositoryDefaults.UserId.System : model.ModifiedBy;
                    Table.Update(model);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    data.Add(model);
                }
            }
            else
            {
                model.CreatedOn = DateTime.Now;
                model.CreatedBy = model.CreatedBy == Guid.Empty ? RepositoryDefaults.UserId.System : model.CreatedBy;
                model.ModifiedBy = RepositoryDefaults.UserId.System;
                model.ModifiedOn = DateTime.Now;
                model.StatusId = (int)EntityStatus.Active;
                EntityEntry<T> entityEntry = await Table.AddAsync(model);

                if (entityEntry.State != EntityState.Added)
                {
                    result.Success = false;
                    data.Add(model);
                }
            }

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<RepositoryResult<T>> DeleteAsync(Guid id, Guid deletedBy, bool hardDelete = false)
        {
            var model = await Table.FirstOrDefaultAsync(x => x.Id == id);

            var data = new List<T>();

            var result = new RepositoryResult<T>()
            {
                Success = true,
                Message = "Ok",
                Data = data
            };
            if (model != null)
            {
                if (hardDelete)
                {
                    result.Message = "hard delete";

                    EntityEntry<T> entityEntry = Table.Remove(model);

                    if (entityEntry.State != EntityState.Deleted)
                    {
                        result.Success = false;
                        data.Add(model);
                        result.Data = data;
                    }
                }
                else
                {
                    result.Message = "status id delete";

                    model.StatusId = (int)EntityStatus.Deleted;
                    model.ModifiedOn = DateTime.Now;
                    model.ModifiedBy = deletedBy;

                    try
                    {
                        Table.Update(model);
                    }
                    catch (Exception ex)
                    {
                        data.Add(model);
                        result.Success = false;
                        result.Message = ex.Message;
                        result.Data = data;
                    }
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Kayıt bulunamadı";
            }

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<RepositoryResult<T>> DeleteAsync(List<Guid> idList, Guid deletedBy, bool hardDelete = false)
        {
            var data = new List<T>();

            var result = new RepositoryResult<T>()
            {
                Success = true,
                Message = "Ok",
                Data = data
            };

            foreach (var id in idList)
            {
                var innerResult = await DeleteAsync(id, deletedBy, hardDelete);
                if (!innerResult.Success)
                {
                    result.Success = false;
                    result.Message = innerResult.Message;
                    data.Add(innerResult.Data.First());
                }
            }
            result.Data = data;

            await _context.SaveChangesAsync();

            return result;
        }

        #endregion

        #region ReadRepository
        public IQueryable<T> FindAll(Expression<Func<T, bool>> method, bool onlyActiveMembers = true, bool tracking = true)
        {
            if (onlyActiveMembers)
                method = method.And(x => x.StatusId == (int)EntityStatus.Active);


            var query = Table.Where(method);

            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }
        #endregion
    }
}
