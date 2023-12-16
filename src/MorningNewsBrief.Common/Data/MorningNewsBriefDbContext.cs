using Microsoft.EntityFrameworkCore;

namespace MorningNewsBrief.Common.Data {
    public class MorningNewsBriefDbContext : DbContext {
        public MorningNewsBriefDbContext(DbContextOptions<MorningNewsBriefDbContext> options) : base(options) {
#if DEBUG
            if (Database.EnsureCreated()) {
                this.Seed();
            }
#endif
        }
    }
}
