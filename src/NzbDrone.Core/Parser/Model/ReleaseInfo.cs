using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using NzbDrone.Core.Download.Pending;
using NzbDrone.Core.Languages;

namespace NzbDrone.Core.Parser.Model
{
    public class ReleaseInfo
    {
        public ReleaseInfo()
        {
            Languages = new List<Language>();
        }

        public string Guid { get; set; }
        public string Title { get; set; }
        public long Size { get; set; }
        public string DownloadUrl { get; set; }
        public string InfoUrl { get; set; }
        public string CommentUrl { get; set; }
        public int IndexerId { get; set; }
        public string Indexer { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int IndexerPriority { get; set; }
        public string DownloadProtocol { get; set; }
        public DateTime PublishDate { get; set; }

        public string Origin { get; set; }
        public string Source { get; set; }
        public string Container { get; set; }
        public string Codec { get; set; }
        public string Resolution { get; set; }

        public List<Language> Languages { get; set; }

        [JsonIgnore]
        public IndexerFlags IndexerFlags { get; set; }

        // Used to track pending releases that are being reprocessed
        [JsonIgnore]
        public PendingReleaseReason? PendingReleaseReason { get; set; }

        public int Age
        {
            get { return DateTime.UtcNow.Subtract(PublishDate).Days; }
            private set { }
        }

        public double AgeHours
        {
            get { return DateTime.UtcNow.Subtract(PublishDate).TotalHours; }
            private set { }
        }

        public double AgeMinutes
        {
            get { return DateTime.UtcNow.Subtract(PublishDate).TotalMinutes; }
            private set { }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} [{2}]", PublishDate, Title, Size);
        }

        public virtual string ToString(string format)
        {
            switch (format.ToUpperInvariant())
            {
                case "L": // Long format
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Guid: " + Guid ?? "Empty");
                    stringBuilder.AppendLine("Title: " + Title ?? "Empty");
                    stringBuilder.AppendLine("Size: " + Size ?? "Empty");
                    stringBuilder.AppendLine("InfoUrl: " + InfoUrl ?? "Empty");
                    stringBuilder.AppendLine("DownloadUrl: " + DownloadUrl ?? "Empty");
                    stringBuilder.AppendLine("Indexer: " + Indexer ?? "Empty");
                    stringBuilder.AppendLine("CommentUrl: " + CommentUrl ?? "Empty");
                    stringBuilder.AppendLine("DownloadProtocol: " + DownloadProtocol ?? "Empty");
                    stringBuilder.AppendLine("PublishDate: " + PublishDate ?? "Empty");
                    return stringBuilder.ToString();
                default:
                    return ToString();
            }
        }
    }

    [Flags]
    public enum IndexerFlags
    {
        Freeleech = 1, // General
        Halfleech = 2, // General, only 1/2 of download counted
        DoubleUpload = 4, // General
        Internal = 8, // General, uploader is an internal release group
        Scene = 16, // General, the torrent comes from a "scene" group
        Freeleech75 = 32, // Signifies a torrent counts towards 75 percent of your download quota.
        Freeleech25 = 64, // Signifies a torrent counts towards 25 percent of your download quota.
    }
}
