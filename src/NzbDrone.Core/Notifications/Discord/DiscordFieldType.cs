namespace NzbDrone.Core.Notifications.Discord
{
    public enum DiscordGrabFieldType
    {
        Overview,
        Rating,
        Genres,
        Quality,
        Group,
        Size,
        Links,
        Release,
        Poster,
        Fanart,
        Indexer
    }

    public enum DiscordImportFieldType
    {
        Overview,
        Rating,
        Genres,
        Quality,
        Group,
        Size,
        Links,
        Release,
        Poster,
        Fanart
    }

    public enum DiscordManualInteractionFieldType
    {
        Overview,
        Rating,
        Genres,
        Quality,
        Group,
        Size,
        Links,
        DownloadTitle,
        Poster,
        Fanart
    }
}
