using AutoMapper;
using Core.Entities;
using GT.Agreement;
using GT.Agreement.Models;
using Services.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Repositories
{
    public class {$EntityName}Repository : Repository<{$EntityName}, {$EntityName}AddDto, {$EntityName}UpdateDto, {$EntityName}Filter, {$EntityName}Dto>
    {
        public {$EntityName}Repository({$ContextName} context, IMapper mapper) : base(context, mapper)
        {
        }

        public override Task<PageResult<{$EntityName}Dto>> GetListWithPageAsync({$EntityName}Filter filter)
        {
            //_query = _query.Where(q => true);
            return base.GetListWithPageAsync(filter);
        }
    }
}
