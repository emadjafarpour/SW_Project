using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;

namespace BitakShop.Infrastructure.Repositories
{
    public class SubFeaturesRepository : BaseRepository<SubFeature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public SubFeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<SubFeature> GetSubFeatures(int featureId)
        {
            return _context.SubFeatures.Where(h => h.FeatureId == featureId & h.IsDeleted == false).ToList();
        }
        public string GetFeatureName(int featureId)
        {
            return _context.Features.Find(featureId).Title;
        }
    }
}
