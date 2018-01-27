using System.Collections.Generic;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.ImportLists
{
    public interface IImportList : IProvider
    {
        IList<ImportListItemInfo> Fetch();
    }
}
