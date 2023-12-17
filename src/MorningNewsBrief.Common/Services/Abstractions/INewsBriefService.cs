using Indice.Types;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;

namespace MorningNewsBrief.Common.Services.Abstractions {
    public interface INewsBriefService {
        public Task<NewsBriefing> GetNewsBriefing(ListOptions<NewsBriefingFilter> options, CancellationToken cancellationToken);
    }
}
