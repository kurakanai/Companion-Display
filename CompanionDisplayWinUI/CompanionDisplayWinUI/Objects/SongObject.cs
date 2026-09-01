using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.ClassImplementations;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Control;
using static CompanionDisplayWinUI.MediaPlayerWidget;

namespace CompanionDisplayWinUI.Objects
{
    class SongObject
    {
        public string title, artist, album, albumCoverUrl, releaseDate, nonTimedLyrics, internationalID, spotifyID;
        public bool hasAlbumUrl = false;
        public bool hasReleaseDate = false;
        public string[] timedLyricsText;
        public double[] timedLyricsTimestamps;
        public int lyricsType = 0;
        public ImageSource albumCover;
        private GlobalSystemMediaTransportControlsSessionMediaProperties internalAudioProperties = MusicAPI.sessionMediaProperties;
        private static readonly SemaphoreSlim semaphore = new(1);
        public SongObject()
        {
            try
            {
                title = internalAudioProperties.Title;
                artist = internalAudioProperties.Artist;
                album = internalAudioProperties.AlbumTitle;
                setAlbumCover("");
            }
            catch { }
        }
        public void setReleaseDate (string releaseDate)
        {
            if(releaseDate != null && releaseDate != "")
            {
                this.releaseDate = releaseDate;
                hasReleaseDate = true;
            }
        }
        public void setAlbumCover(string albumCoverUrl)
        {
            CommonlyAccessedInstances.mainDispatcher.TryEnqueue(() =>
            {
                if (albumCoverUrl != "")
                {
                    this.albumCoverUrl = albumCoverUrl;
                    hasAlbumUrl = true;
                }
                if (albumCover == null)
                {
                    if(albumCoverUrl != ""){
                        albumCover = new BitmapImage(new Uri(albumCoverUrl));
                    }
                    else
                    {
                        albumCover = Helper.GetThumbnail(MusicAPI.sessionMediaProperties.Thumbnail);
                    }
                    semaphore.Release();
                    MusicAPI.SongCoverChanged();
                }
            });
        }
        public bool checkIfSame(SongObject other)
        {
            return other.title == this.title && other.album == this.album;
        }
        public bool checkIfSameSimple(string title, string album) => title == this.title && album == this.album;
        public void setLyricsType()
        {
            if(timedLyricsText != null)
            {
                lyricsType = 2;
            }
            else if(nonTimedLyrics != null)
            {
                lyricsType = 1;
            }
        }
    }
}
