using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Notifications.Xbmc;
using NzbDrone.Core.Notifications.Xbmc.Model;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Music;

namespace NzbDrone.Core.Test.NotificationTests.Xbmc.Json
{
    [TestFixture]
    public class UpdateFixture : CoreTest<JsonApiProvider>
    {
        private const string MB_ID = "5";
        private XbmcSettings _settings;
        private List<KodiArtist> _xbmcArtist;

        [SetUp]
        public void Setup()
        {
            _settings = Builder<XbmcSettings>.CreateNew()
                                             .Build();

            _xbmcArtist = Builder<KodiArtist>.CreateListOfSize(3)
                                         .TheFirst(1)
                                         .With(s => s.MusicbrainzArtistId = new List<string> { MB_ID.ToString()})
                                         .Build()
                                         .ToList();

            Mocker.GetMock<IXbmcJsonApiProxy>()
                  .Setup(s => s.GetArtist(_settings))
                  .Returns(_xbmcArtist);

            Mocker.GetMock<IXbmcJsonApiProxy>()
                  .Setup(s => s.GetActivePlayers(_settings))
                  .Returns(new List<ActivePlayer>());
        }

        [Test]
        public void should_update_using_artist_path()
        {
            var artist = Builder<Music.Artist>.CreateNew()
                                        .With(s => s.ForeignArtistId = MB_ID)
                                        .Build();

            Subject.Update(_settings, artist);

            Mocker.GetMock<IXbmcJsonApiProxy>()
                  .Verify(v => v.UpdateLibrary(_settings, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void should_update_all_paths_when_artist_path_not_found()
        {
            var fakeArtist = Builder<Music.Artist>.CreateNew()
                                            .With(s => s.ForeignArtistId = "1000")
                                            .With(s => s.Name = "Not 30 Rock")
                                            .Build();

             Subject.Update(_settings, fakeArtist);

             Mocker.GetMock<IXbmcJsonApiProxy>()
                   .Verify(v => v.UpdateLibrary(_settings, null), Times.Once());
        }
    }
}
