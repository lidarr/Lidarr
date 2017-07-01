﻿using System.Linq;
using NzbDrone.Core.Datastore;
using System.Collections.Generic;
using NzbDrone.Core.Messaging.Events;
using Marr.Data.QGen;

namespace NzbDrone.Core.Music
{
    public interface IAlbumRepository : IBasicRepository<Album>
    {
        bool AlbumPathExists(string path);
        List<Album> GetAlbums(int artistId);
        Album FindByName(string cleanTitle);
        Album FindByArtistAndName(string artistName, string cleanTitle);
        Album FindById(string spotifyId);
        void SetMonitoredFlat(Album album, bool monitored);
    }

    public class AlbumRepository : BasicRepository<Album>, IAlbumRepository
    {
        public AlbumRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
       

        public bool AlbumPathExists(string path)
        {
            return Query.Where(c => c.Path == path).Any();
        }
        public List<Album> GetAlbums(int artistId)
        {
            return Query.Where(s => s.ArtistId == artistId).ToList();
        }

        public Album FindById(string foreignAlbumId)
        {
            return Query.Where(s => s.ForeignAlbumId == foreignAlbumId).SingleOrDefault();
        }

        public void SetMonitoredFlat(Album album, bool monitored)
        {
            album.Monitored = monitored;
            SetFields(album, p => p.Monitored);
        }

        public Album FindByName(string cleanTitle)
        {
            cleanTitle = cleanTitle.ToLowerInvariant();

            return Query.Where(s => s.CleanTitle == cleanTitle)
                        .SingleOrDefault();
        }

        public Album FindByArtistAndName(string artistName, string cleanTitle)
        {
            var cleanArtistName = Parser.Parser.CleanArtistTitle(artistName);
            cleanTitle = cleanTitle.ToLowerInvariant();
            var query = Query.Join<Album, Artist>(JoinType.Inner, album => album.Artist, (album, artist) => album.ArtistId == artist.Id)
                        .Where<Artist>(artist => artist.CleanName == cleanArtistName)
                        .Where<Album>(album => album.CleanTitle == cleanTitle);
            return Query.Join<Album, Artist>(JoinType.Inner, album => album.Artist, (album, artist) => album.ArtistId == artist.Id )
                        .Where<Artist>(artist => artist.CleanName == cleanArtistName)
                        .Where<Album>(album => album.CleanTitle == cleanTitle)
                        .SingleOrDefault();
        }
    }
}
