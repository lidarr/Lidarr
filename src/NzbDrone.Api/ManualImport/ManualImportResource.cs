using System.Collections.Generic;
using System.Linq;
using NzbDrone.Api.Episodes;
using NzbDrone.Api.REST;
using NzbDrone.Api.Series;
using NzbDrone.Api.Albums;
using NzbDrone.Api.Music;
using NzbDrone.Api.Tracks;
using NzbDrone.Common.Crypto;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.Core.Qualities;

namespace NzbDrone.Api.ManualImport
{
    public class ManualImportResource : RestResource
    {
        public string Path { get; set; }
        public string RelativePath { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public ArtistResource Artist { get; set; }
        public AlbumResource Album { get; set; }
        public List<TrackResource> Tracks { get; set; }
        public QualityModel Quality { get; set; }
        public int QualityWeight { get; set; }
        public string DownloadId { get; set; }
        public IEnumerable<Rejection> Rejections { get; set; }
    }

    public static class ManualImportResourceMapper
    {
        public static ManualImportResource ToResource(this Core.MediaFiles.TrackImport.Manual.ManualImportItem model)
        {
            if (model == null) return null;

            return new ManualImportResource
            {
                Id = HashConverter.GetHashInt31(model.Path),

                Path = model.Path,
                RelativePath = model.RelativePath,
                Name = model.Name,
                Size = model.Size,
                Artist = model.Artist.ToResource(),
                Album = model.Album.ToResource(),
                Tracks = model.Tracks.ToResource(),
                Quality = model.Quality,
                //QualityWeight
                DownloadId = model.DownloadId,
                Rejections = model.Rejections
            };
        }

        public static List<ManualImportResource> ToResource(this IEnumerable<Core.MediaFiles.TrackImport.Manual.ManualImportItem> models)
        {
            return models.Select(ToResource).ToList();
        }
    }
}
