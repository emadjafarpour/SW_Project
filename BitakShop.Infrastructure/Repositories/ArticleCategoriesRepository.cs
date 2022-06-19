using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infrastructure.Repositories
{
    public class ArticleCategoriesRepository : BaseRepository<ArticleCategory, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ArticleCategoriesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Article> GetCategoryArticles(Article article, int skip=0, int take = 5)
        {
            return _context.Articles.Where(a => a.IsDeleted == false && a.ArticleCategoryId == article.ArticleCategoryId && a.Id != article.Id)
                .Include(a => a.User)
                .Include(a => a.ArticleCategory)
                .OrderBy(a => a.InsertDate).Skip(0).Take(5).ToList();
        }

    }
}
