﻿using Espera.View.Properties;
using Espera.View.ViewModels;
using Ionic.Utils;
using MahApps.Metro;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ListView = System.Windows.Controls.ListView;

namespace Espera.View
{
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");

            this.ChangeColor(Settings.Default.AccentColor);
        }

        private void AddSongsButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialogEx
            {
                Description = "Choose a folder containing the music that you want to add to the library"
            };

            dialog.ShowDialog();

            string selectedPath = dialog.SelectedPath;

            if (!String.IsNullOrEmpty(selectedPath))
            {
                this.shellViewModel.LocalViewModel.AddSongs(selectedPath);
            }
        }

        private void BlueColorButtonButtonClick(object sender, RoutedEventArgs e)
        {
            this.ChangeColor("Blue");
            Settings.Default.AccentColor = "Blue";
        }

        private void ChangeColor(string color)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(accent => accent.Name == color), Theme.Dark);
        }

        private void CreateAdminButtonClick(object sender, RoutedEventArgs e)
        {
            ICommand command = this.shellViewModel.AdministratorViewModel.CreateAdminCommand;

            if (command.CanExecute(null))
            {
                command.Execute(null);
            }

            this.adminPasswordBox.Password = String.Empty;
        }

        private void CreationPasswordChanged(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.AdministratorViewModel.CreationPassword = ((PasswordBox)sender).Password;
        }

        private void GreenColorButtonButtonClick(object sender, RoutedEventArgs e)
        {
            this.ChangeColor("Green");
            Settings.Default.AccentColor = "Green";
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            ICommand command = this.shellViewModel.AdministratorViewModel.LoginCommand;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }

            this.loginPasswordBox.Password = String.Empty;
        }

        private void LoginPasswordChanged(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.AdministratorViewModel.LoginPassword = ((PasswordBox)sender).Password;
        }

        private void MainWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            // We want to lose the focus of a textbox when the user clicks anywhere in the application
            this.mainGrid.Focus();
        }

        private void MetroWindowClosing(object sender, CancelEventArgs e)
        {
            this.shellViewModel.Dispose();
        }

        private void MetroWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (this.shellViewModel.IsPlaying)
                {
                    if (this.shellViewModel.PauseCommand.CanExecute(null))
                    {
                        this.shellViewModel.PauseCommand.Execute(null);
                    }
                }

                else if (this.shellViewModel.PlayCommand.CanExecute(null))
                {
                    this.shellViewModel.PlayCommand.Execute(false);
                }
            }
        }

        private void PlaylistContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (((ListView)sender).Items.IsEmpty)
            {
                e.Handled = true;
            }
        }

        private void PlaylistDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.shellViewModel.PlayCommand.CanExecute(null))
            {
                this.shellViewModel.PlayCommand.Execute(true);
            }
        }

        private void PlaylistKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (this.shellViewModel.RemoveSelectedPlaylistEntriesCommand.CanExecute(null))
                {
                    this.shellViewModel.RemoveSelectedPlaylistEntriesCommand.Execute(null);
                }
            }

            else if (e.Key == Key.Enter)
            {
                if (this.shellViewModel.PlayCommand.CanExecute(null))
                {
                    this.shellViewModel.PlayCommand.Execute(true);
                }
            }
        }

        private void PlaylistNameTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((TextBox)sender);

            textBox.CaretIndex = textBox.Text.Length;
        }

        private void PlaylistNameTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.shellViewModel.CurrentEditedPlaylist.EditName = false;
            }

            e.Handled = true; // Don't send key events when renaming a playlist
        }

        private void PlaylistNameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PlaylistViewModel playlist = this.shellViewModel.CurrentEditedPlaylist;

            if (playlist != null)
            {
                playlist.EditName = false;
            }
        }

        private void PlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.shellViewModel.SelectedPlaylistEntries = ((ListView)sender).SelectedItems.Cast<PlaylistEntryViewModel>();
        }

        private void PlaylistsKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var command = this.shellViewModel.RemovePlaylistCommand;

                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }

        private void PurpleColorButtonButtonClick(object sender, RoutedEventArgs e)
        {
            this.ChangeColor("Purple");
            Settings.Default.AccentColor = "Purple";
        }

        private void RedColorButtonButtonClick(object sender, RoutedEventArgs e)
        {
            this.ChangeColor("Red");
            Settings.Default.AccentColor = "Red";
        }

        private void SearchTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.shellViewModel.YoutubeViewModel.StartSearch();
            }

            e.Handled = true;
        }

        private void SongDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ICommand addToPlaylist = this.shellViewModel.CurrentSongSource.AddToPlaylistCommand;

            if (e.LeftButton == MouseButtonState.Pressed && addToPlaylist.CanExecute(null))
            {
                addToPlaylist.Execute(null);
            }
        }

        private void SongListContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (((ListView)sender).Items.IsEmpty)
            {
                e.Handled = true;
            }
        }

        private void SongListKeyUp(object sender, KeyEventArgs e)
        {
            ICommand removeFromLibrary = this.shellViewModel.LocalViewModel.RemoveFromLibraryCommand;

            if (e.Key == Key.Delete)
            {
                if (removeFromLibrary.CanExecute(null))
                {
                    removeFromLibrary.Execute(null);
                }
            }
        }

        private void SongListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.shellViewModel.CurrentSongSource.SelectedSongs = ((ListView)sender).SelectedItems.Cast<SongViewModelBase>();
        }

        private void SortLocalSongAlbum(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.LocalViewModel.OrderByAlbum();
        }

        private void SortLocalSongArtist(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.LocalViewModel.OrderByArtist();
        }

        private void SortLocalSongDuration(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.LocalViewModel.OrderByDuration();
        }

        private void SortLocalSongGenre(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.LocalViewModel.OrderByGenre();
        }

        private void SortLocalSongTitle(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.LocalViewModel.OrderByTitle();
        }

        private void SortYoutubeSongDuration(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.YoutubeViewModel.OrderByDuration();
        }

        private void SortYoutubeSongRating(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.YoutubeViewModel.OrderByRating();
        }

        private void SortYoutubeSongTitle(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.YoutubeViewModel.OrderByTitle();
        }

        private void SortYoutubeSongViews(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.YoutubeViewModel.OrderByViews();
        }
    }
}