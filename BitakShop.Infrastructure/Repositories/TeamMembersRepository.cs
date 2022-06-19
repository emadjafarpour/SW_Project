using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infrastructure.Repositories
{
    public class TeamMembersRepository : BaseRepository<TeamMember, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public TeamMembersRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<TeamMember> GetTeamMembers()
        {
            return _context.TeamMembers.Where(m => m.IsDeleted == false).ToList();
        }
    }
}
