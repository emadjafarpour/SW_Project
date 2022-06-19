using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;

namespace BitakShop.Infrastructure.Repositories
{
    public class FeaturesRepository : BaseRepository<Feature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public FeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Feature> GetAllFeatures()
        {
            return _context.Features.Include(f => f.SubFeatures).Where(f => f.IsDeleted == false && f.SubFeatures.Any()).ToList();
        }
        public List<Feature> GetAllGroupFeatures(int groupId)
        {
            var pgFeatures = _context.ProductGroupFeatures.Where(f => f.ProductGroupId == groupId).ToList();
            var features = new List<Feature>();
            foreach (var feature in pgFeatures)
                features.Add(_context.Features.Include(f=>f.SubFeatures).FirstOrDefault(f=>f.Id == feature.FeatureId));
            return features;
        }
    }
}
