using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Extras.Files;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Music;

namespace NzbDrone.Core.Extras
{
    public interface IExtraService
    {
        void ImportExtraFiles(string sourceFolder, TrackFile trackFile, bool isReadOnly);
    }

    public class ExtraService : IExtraService,
                                IHandle<MediaCoversUpdatedEvent>,
                                IHandle<TrackFolderCreatedEvent>,
                                IHandle<ArtistScannedEvent>,
                                IHandle<ArtistRenamedEvent>
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly IAlbumService _albumService;
        private readonly ITrackService _trackService;
        private readonly IDiskProvider _diskProvider;
        private readonly IConfigService _configService;
        private readonly List<IManageExtraFiles> _extraFileManagers;
        private readonly Logger _logger;

        public ExtraService(IMediaFileService mediaFileService,
                            IAlbumService albumService,
                            ITrackService trackService,
                            IDiskProvider diskProvider,
                            IConfigService configService,
                            IEnumerable<IManageExtraFiles> extraFileManagers,
                            Logger logger)
        {
            _mediaFileService = mediaFileService;
            _albumService = albumService;
            _trackService = trackService;
            _diskProvider = diskProvider;
            _configService = configService;
            _extraFileManagers = extraFileManagers.OrderBy(e => e.Order).ToList();
            _logger = logger;
        }

        public void ImportExtraFiles(string sourceFolder, TrackFile trackFile, bool isReadOnly)
        {
            if (!_configService.ImportExtraFiles)
            {
                return;
            }

            var sourceFileName = Path.GetFileNameWithoutExtension(trackFile.Path);
            var files = _diskProvider.GetFiles(sourceFolder, false);

            var wantedExtensions = _configService.ExtraFileExtensions
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim(' ', '.'))
                .ToList();

            var matchingFilenames = files.Where(f =>
                Path.GetFileNameWithoutExtension(f)
                    .StartsWith(sourceFileName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            var filteredFilenames = new List<string>();
            var hasNfo = false;

            foreach (var matchingFilename in matchingFilenames)
            {
                // Filter out duplicate NFO files
                if (matchingFilename.EndsWith(".nfo", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (hasNfo)
                    {
                        continue;
                    }

                    hasNfo = true;
                }

                filteredFilenames.Add(matchingFilename);
            }

            foreach (var matchingFilename in filteredFilenames)
            {
                var matchingExtension = wantedExtensions.FirstOrDefault(e => matchingFilename.EndsWith(e));

                if (matchingExtension == null)
                {
                    continue;
                }

                try
                {
                    foreach (var extraFileManager1 in _extraFileManagers)
                    {
                        var extension = Path.GetExtension(matchingFilename);
                        var extraFile = extraFileManager1.Import(trackFile.Artist, trackFile, matchingFilename, extension, isReadOnly);

                        if (extraFile != null)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to import extra file: {0}", matchingFilename);
                }
            }
        }

        public void Handle(MediaCoversUpdatedEvent message)
        {
            if (message.Updated)
            {
                var artist = message.Artist ?? message.Album.Artist;

                foreach (var extraFileManager in _extraFileManagers)
                {
                    extraFileManager.CreateAfterMediaCoverUpdate(artist);
                }
            }
        }

        public void Handle(ArtistScannedEvent message)
        {
            var artist = message.Artist;

            var trackFiles = GetTrackFiles(artist.Id);

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.CreateAfterArtistScan(artist, trackFiles);
            }
        }

        public void Handle(TrackFolderCreatedEvent message)
        {
            var artist = message.Artist;
            var album = _albumService.GetAlbum(message.TrackFile.AlbumId);

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.CreateAfterTrackFolder(artist, album, message.ArtistFolder, message.AlbumFolder);
            }
        }

        public void Handle(ArtistRenamedEvent message)
        {
            var artist = message.Artist;
            var trackFiles = GetTrackFiles(artist.Id);

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.MoveFilesAfterRename(artist, trackFiles);
            }
        }

        private List<TrackFile> GetTrackFiles(int artistId)
        {
            var trackFiles = _mediaFileService.GetFilesByArtist(artistId);
            var tracks = _trackService.GetTracksByArtist(artistId);

            foreach (var trackFile in trackFiles)
            {
                var localTrackFile = trackFile;
                trackFile.Tracks = tracks.Where(e => e.TrackFileId == localTrackFile.Id).ToList();
            }

            return trackFiles;
        }
    }
}
