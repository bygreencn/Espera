﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using TagLib;
using File = TagLib.File;

namespace Espera.Core
{
    /// <summary>
    /// Encapsulates a recursive call through the local filesystem that reads the tags of all WAV
    /// and MP3 files and returns them.
    /// </summary>
    internal sealed class LocalSongFinder : ILocalSongFinder, IEnableLogger
    {
        private static readonly string[] AllowedExtensions = { ".mp3", ".wav", ".m4a", ".aac" };
        private readonly string directoryPath;
        private readonly IFileSystem fileSystem;

        public LocalSongFinder(string directoryPath, IFileSystem fileSystem = null)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            this.directoryPath = directoryPath;
            this.fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// This method scans the directory, specified in the constructor, and returns an observable
        /// with a tuple that contains the song and the data of the artwork.
        /// </summary>
        public IObservable<Tuple<LocalSong, byte[]>> GetSongsAsync()
        {
            return this.ScanDirectoryForValidPaths(this.directoryPath)
                .Select(this.ProcessFile)
                .Where(t => t != null)
                .ToObservable(RxApp.TaskpoolScheduler);
        }

        private static Tuple<LocalSong, byte[]> CreateSong(Tag tag, TimeSpan duration, string filePath)
        {
            var song = new LocalSong(filePath, duration)
            {
                Album = PrepareTag(tag.Album, String.Empty),
                Artist = PrepareTag(tag.FirstAlbumArtist ?? tag.FirstPerformer, "Unknown Artist"), //HACK: In the future retrieve the string for an unkown artist from the view if we want to localize it
                Genre = PrepareTag(tag.FirstGenre, String.Empty),
                Title = PrepareTag(tag.Title, Path.GetFileNameWithoutExtension(filePath)),
                TrackNumber = (int)tag.Track
            };

            IPicture picture = tag.Pictures.FirstOrDefault();

            return Tuple.Create(song, picture?.Data.Data);
        }

        private static string PrepareTag(string tag, string replacementIfNull)
        {
            return tag == null ? replacementIfNull : TagSanitizer.Sanitize(tag);
        }

        private Tuple<LocalSong, byte[]> ProcessFile(string filePath)
        {
            try
            {
                using (var fileAbstraction = new TagLibFileAbstraction(filePath, this.fileSystem))
                {
                    using (var file = File.Create(fileAbstraction))
                    {
                        if (file?.Tag != null)
                        {
                            return CreateSong(file.Tag, file.Properties.Duration, file.Name);
                        }

                        return null;
                    }
                }
            }

            catch (Exception ex)
            {
                this.Log().ErrorException("Couldn't read song file \{filePath}", ex);
                return null;
            }
        }

        private IEnumerable<string> ScanDirectoryForValidPaths(string rootPath)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();

            try
            {
                files = this.fileSystem.Directory.GetFiles(rootPath)
                     .Where(x => AllowedExtensions.Contains(Path.GetExtension(x).ToLowerInvariant()));
            }

            catch (Exception ex)
            {
                this.Log().ErrorException("Couldn't get files from directory \{rootPath}", ex);
            }

            IEnumerable<string> directories = Enumerable.Empty<string>();

            try
            {
                directories = this.fileSystem.Directory.GetDirectories(rootPath);
            }

            catch (Exception ex)
            {
                this.Log().ErrorException("Couldn't get directories from directory \{rootPath}", ex);
            }

            return files.Concat(directories.SelectMany(ScanDirectoryForValidPaths));
        }

        private class TagLibFileAbstraction : File.IFileAbstraction, IDisposable
        {
            public TagLibFileAbstraction(string path, IFileSystem fileSystem)
            {
                if (path == null)
                    throw new ArgumentNullException(nameof(path));

                if (fileSystem == null)
                    throw new ArgumentNullException(nameof(fileSystem));

                this.Name = path;

                Stream stream = fileSystem.File.OpenRead(path);

                this.ReadStream = stream;
                this.WriteStream = stream;
            }

            public string Name { get; }

            public Stream ReadStream { get; }

            public Stream WriteStream { get; }

            public void CloseStream(Stream stream) => stream.Close();

            public void Dispose() => this.ReadStream.Dispose();
        }
    }
}