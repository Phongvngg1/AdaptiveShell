﻿using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Shell.Controls {
    public sealed partial class StartScreenControl : UserControl {
        public ApplicationManager ApplicationManager { get; set; }
        public Action ToggleVisibility { get; set; }
        public Action OnExit { get; set; }
        public Action OnFocusLost { get; set; }
        public Double ScreenWidth;
        public Double ScreenHeight;

        public StartScreenControl() {
            this.InitializeComponent();
        }

        private async void Control_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set wallpaper
            var background = await Shell.PersonalizationLibrary.BackgroundImageManager.GetBackgroundImage();
            if (background != null)
                this.Root.Background = new ImageBrush() {
                    ImageSource = background,
                    Stretch = Stretch.UniformToFill
                };

            // Set profile
            try {
                var users = await Windows.System.User.FindAllAsync();
                var user = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                            p.Type == UserType.LocalUser).FirstOrDefault();

                var userPicure = await user.GetPictureAsync(Windows.System.UserPictureSize.Size208x208);
                var contact = new Windows.ApplicationModel.Contacts.Contact { };
                contact.SourceDisplayPicture = userPicure;
                this.ProfilePicture.Contact = contact;
            } catch { }
        }

        public void Control_OnReady() {
            Debug.WriteLine("StartScreenControl OnReady!");

            if (this.ApplicationManager == null) return;

            this.LiveTilesLayout.ScreenHeight = this.ScreenHeight;
            this.LiveTilesLayout.ScreenWidth = this.ScreenWidth;
            this.LiveTilesLayout.ItemsSource = this.ApplicationManager.LiveTiles;
            this.LiveTilesLayout.Control_OnReady();

            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.LiveTilesLayout.ScreenHeight = this.ScreenHeight;
            this.LiveTilesLayout.ScreenWidth = this.ScreenWidth;

            if (this.ScreenWidth <= 950) {
                this.StartHeaderToolbar.Padding = new Thickness(0);

                this.StartScreenLayout.Height = Double.NaN;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.AppsListLayout.Height = Double.NaN;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.AppsListLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Horizontal;
            } else {
                Double padding = this.ScreenWidth * 0.025;
                this.StartHeaderToolbar.Padding = new Thickness(padding, this.ScreenHeight * 0.05, padding, 0);

                this.StartScreenLayout.Height = this.ScreenHeight;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.StartScreenLayout.Height = this.ScreenHeight;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Vertical;
            }

            // Force update
            this.Root.UpdateLayout();
        }

        private void ScrollViewer_ViewChanging(Object sender, ScrollViewerViewChangingEventArgs e) {
            Int32 MAX_DARK = 150;

            try {
                if (this.ScreenWidth <= 950) {
                    if (((ScrollViewer)sender).HorizontalOffset == e.NextView.HorizontalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                        ), 0, 0, 0)
                    };
                } else {
                    if (((ScrollViewer)sender).VerticalOffset == e.NextView.VerticalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                        ), 0, 0, 0)
                    };
                }
            } catch { }
        }

        private void ExitBtn_Click(Object sender, RoutedEventArgs e) {
            if (this.OnExit == null) return;
            this.OnExit();
        }

        private void UserControl_FocusDisengaged(Control sender, FocusDisengagedEventArgs args) {
            if (this.OnFocusLost == null) return;

            this.OnFocusLost();
        }
    }
}