using UserManagementService.Data;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Services
{
    internal class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PersistedGrantStore(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await _applicationDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).ToListAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await _applicationDbContext.PersistedGrants.FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var grants = await _applicationDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId && x.ClientId == clientId).ToListAsync();
            foreach(var grant in grants)
            {
                _applicationDbContext.PersistedGrants.Remove(grant);
            }
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var grants = await _applicationDbContext.PersistedGrants.Where(x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type).ToListAsync();
            foreach (var grant in grants)
            {
                _applicationDbContext.PersistedGrants.Remove(grant);
            }
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string key)
        {
            var grants = await _applicationDbContext.PersistedGrants.Where(x => x.Key == key).ToListAsync();
            foreach (var grant in grants)
            {
                _applicationDbContext.PersistedGrants.Remove(grant);
            }
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            _applicationDbContext.PersistedGrants.Add(grant);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
