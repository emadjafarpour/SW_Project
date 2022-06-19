using BitakShop.Core;
using BitakShop.Core.Models;
using BitakShop.Infrastructure.Filters;
using BitakShop.Infrastructure.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BitakShop.Infrastructure
{
    public interface IBaseRepository<T> where T : class, IBaseEntity
    {
        List<T> GetAll();
        List<T> GetSome(PaginationFilter pagination);
        int GetCount();
        T Get(int id);
        T Add(T entity);
        T Update(T entity);
        T Delete(int id);
    }
    public abstract class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
         where TEntity : class, IBaseEntity
         where TContext : DbContext
    {
        private readonly TContext context;
        private readonly LogsRepository logger;
        public BaseRepository(TContext context, LogsRepository logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public  TEntity Add(TEntity entity)
        {
            var user = GetCurrentUser();
            entity.InsertDate = DateTime.Now;
            entity.InsertUser = user.UserName;
            var ent = entity.GetType().Name;

            context.Set<TEntity>().Add(entity);
             context.SaveChanges();
            logger.LogEvent(entity.GetType().Name, entity.Id,"Add");
            return entity;
        }

        public  TEntity Delete(int id)
        {
            var entity =  context.Set<TEntity>().Find(id);
            if (entity == null)
            {
                return entity;
            }
            var user = GetCurrentUser();
            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = user.UserName;

            entity.IsDeleted = true;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
            logger.LogEvent(entity.GetType().Name, entity.Id, "Delete");
            return entity;
        }

        public  TEntity Get(int id)
        {
            return  context.Set<TEntity>().Find(id);
        }

        public List<TEntity> GetAll()
        {
            return  context.Set<TEntity>().Where(e=>e.IsDeleted == false).OrderByDescending(e=>e.InsertDate).ToList();
        }     
        public List<TEntity> GetSome(PaginationFilter pagination)
        {
            var entity =  context.Set<TEntity>().Skip((pagination.PageNumber - 1) * pagination.PageSize)
               .Take(pagination.PageSize).ToList();

            return entity;
        }
        public int GetCount()
        {
            var entity =  context.Set<TEntity>().Count();

            return entity;
        }
        public  TEntity Update(TEntity entity)
        {
            var user = GetCurrentUser();
            entity.UpdateDate = DateTime.Now;
            if(user != null)
                entity.UpdateUser = user.UserName;

            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
            logger.LogEvent(entity.GetType().Name, entity.Id, "Update");
            return entity;
        }
        public User GetCurrentUser()
        {
            var username = HttpContext.Current.User.Identity.GetUserName();
            if (username == null)
                return null;
            var user = context.Set<User>().FirstOrDefault(u => u.UserName.Trim().ToLower() == username.Trim().ToLower());
            return user;

        }
    }
}
