using Lidarr.Http;
using Lidarr.Http.Extensions;
using NzbDrone.Core.Blocklisting;
using NzbDrone.Core.Datastore;

namespace Lidarr.Api.V1.Blocklist
{
    public class BlocklistModule : LidarrRestModule<BlocklistResource>
    {
        private readonly IBlocklistService _blocklistService;

        public BlocklistModule(IBlocklistService blocklistService)
        {
            _blocklistService = blocklistService;
            GetResourcePaged = GetBlocklist;
            DeleteResource = DeleteBlocklist;

            Delete("/bulk", x => Remove());
        }

        private PagingResource<BlocklistResource> GetBlocklist(PagingResource<BlocklistResource> pagingResource)
        {
            var pagingSpec = pagingResource.MapToPagingSpec<BlocklistResource, NzbDrone.Core.Blocklisting.Blocklist>("date", SortDirection.Descending);

            return ApplyToPage(_blocklistService.Paged, pagingSpec, BlocklistResourceMapper.MapToResource);
        }

        private void DeleteBlocklist(int id)
        {
            _blocklistService.Delete(id);
        }

        private object Remove()
        {
            var resource = Request.Body.FromJson<BlocklistBulkResource>();

            _blocklistService.Delete(resource.Ids);

            return new object();
        }
    }
}
