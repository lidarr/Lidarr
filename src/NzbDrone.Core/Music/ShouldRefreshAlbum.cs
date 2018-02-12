using NLog;
using System;

namespace NzbDrone.Core.Music
{
    public interface ICheckIfAlbumShouldBeRefreshed
    {
        bool ShouldRefresh(Album album);
    }

    public class ShouldRefreshAlbum : ICheckIfAlbumShouldBeRefreshed
    {
        private readonly Logger _logger;

        public ShouldRefreshAlbum(Logger logger)
        {
            _logger = logger;
        }

        public bool ShouldRefresh(Album album)
        {
            if (album.LastInfoSync < DateTime.UtcNow.AddDays(-30))
            {
                _logger.Trace("Album {0} last updated more than 30 days ago, should refresh.", album.Title);
                return true;
            }

            if (album.LastInfoSync >= DateTime.UtcNow.AddHours(-6))
            {
                _logger.Trace("Album {0} last updated less than 6 hours ago, should not be refreshed.", album.Title);
                return false;
            }

            if (album.ReleaseDate > DateTime.UtcNow.AddDays(-30))
            {
                _logger.Trace("album {0} released less than 30 days ago, should refresh.", album.Title);
                return true;
            }

            _logger.Trace("Album {0} released long ago, should not be refreshed.", album.Title);
            return false;
        }
    }
}
