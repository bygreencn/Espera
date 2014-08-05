﻿using Espera.Core;
using Espera.Core.Management;
using Espera.Core.Settings;
using System;
using Rareform.Validation;
using ReactiveUI;

namespace Espera.View.ViewModels
{
    internal class SoundCloudViewModel : NetworkSongViewModel<SoundCloudSongViewModel, SoundCloudSong>
    {
        private readonly ViewSettings viewSettings;
        private SortOrder uploaderOrder;

        public SoundCloudViewModel(Library library, Guid accessToken, CoreSettings coreSettings, ViewSettings viewSettings, INetworkStatus networkStatus = null, INetworkSongFinder<SoundCloudSong> songFinder = null)
            : base(library, accessToken, coreSettings,
                song => new SoundCloudSongViewModel(song), networkStatus, songFinder ?? new SoundCloudSongFinder())
        {
            if (viewSettings == null)
                Throw.ArgumentNullException(() => viewSettings);

            this.viewSettings = viewSettings;

            this.OrderByUploaderCommand = new ReactiveCommand();
            this.OrderByUploaderCommand.Subscribe(_ => this.ApplyOrder(SortHelpers.GetOrderByUploader, ref this.uploaderOrder));
        }

        public int DurationColumnWidth
        {
            get { return this.viewSettings.SoundCloudDurationColumnWidth; }
            set { this.viewSettings.SoundCloudDurationColumnWidth = value; }
        }

        public int LinkColumnWidth
        {
            get { return this.viewSettings.SoundCloudLinkColumnWidth; }
            set { this.viewSettings.SoundCloudLinkColumnWidth = value; }
        }

        public ReactiveCommand OrderByUploaderCommand { get; private set; }

        public int TitleColumnWidth
        {
            get { return this.viewSettings.SoundCloudTitleColumnWidth; }
            set { this.viewSettings.SoundCloudTitleColumnWidth = value; }
        }

        public int UploaderColumnWidth
        {
            get { return this.viewSettings.SoundCloudUploaderColumnWidth; }
            set { this.viewSettings.SoundCloudUploaderColumnWidth = value; }
        }
    }
}