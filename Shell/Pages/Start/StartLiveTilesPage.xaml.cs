﻿using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using static Shell.Pages.StartPage;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    public sealed partial class StartLiveTilesPage : Page {

        private Double ScreenWidth;
        private Double ScreenHeight;
        private StartScrenParameters Arguments;

        public StartLiveTilesPage() {
            this.InitializeComponent();

            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.Arguments = (StartScrenParameters)e.Parameter;
        }

        private void StartLiveTilesPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
            this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;

            if (this.LiveTiles.ItemsPanelRoot == null)
                return;
            
            if (this.ScreenWidth <= 950) {
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Center;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Stretch;

                this.StartScreenScrollViewer.Padding = new Thickness(0);
                this.StartScreenScrollViewer.Margin = new Thickness(0);
                this.AllAppsBtn.Padding = new Thickness(0);
            } else {
                if (this.ScreenHeight <= 1050) {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 6;
                } else {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 8;
                }

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Vertical;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Stretch;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Center;

                this.StartScreenScrollViewer.Padding = new Thickness(this.ScreenWidth * 0.05);
                this.StartScreenScrollViewer.Margin = new Thickness(0, 0, 0, ((this.ScreenWidth * 0.05) * -1) - 14);
                this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.075, 0, this.ScreenWidth * 0.05, this.ScreenWidth * 0.05);
            }
        }

        private void StartLiveTilesPage_OnLoaded(Object sender, RoutedEventArgs e) {
            // Trigger reflow.
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private async void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((PreviewTile)sender).DataContext;

            // Set span.
            var container = this.LiveTiles.ContainerFromItem(item);
            container.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            container.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);

            await item.LiveTile.UpdateAsync();
            if (item.TileData != null) {
                PreviewTileUpdater tileUpdater = item.LiveTile.CreateTileUpdater();
                PreviewBadgeUpdater badgeUpdater = item.LiveTile.CreateBadgeUpdater();

                foreach (var data in item.TileData) {
                    // FIXME: Queue
                    tileUpdater.Update(new TileNotification(data.Payload));
                }
            }

            // Push updates.
            item.LiveTile.UpdateLayout();
            await item.LiveTile.UpdateAsync();

            // TODO: handle background based on tile data,
        }

        private async void LiveTile_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            // TODO
            var item = (TileModel)((Grid)sender).DataContext;
            //item.Launcher();
        }

        private async void LiveTileContext_Click(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((ToggleMenuFlyoutItem)sender).DataContext;
            var tile = item.LiveTile;

            switch (((MenuFlyoutItem)sender).Name) {
                case "SmallOpt":
                    item.Size = TileSize.Small;
                    break;
                case "MediumOpt":
                    item.Size = TileSize.Medium;
                    break;
                case "WideOpt":
                    item.Size = TileSize.Wide;
                    break;
                case "LargeOpt":
                    item.Size = TileSize.Large;
                    break;
            }
            // Set span
            var gridItem = this.LiveTiles.ContainerFromItem(item);
            gridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            gridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);

            // Push updates
            tile.UpdateLayout();
            await tile.UpdateAsync();

            // FIXME: figure out why checkmark doesn't update.
        }

        private void LiveTilesLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private void AllAppsBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            if (this.Arguments == null) return;

            this.Arguments.AllAppsBtnCallback();
        }

        private void LiveTiles_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            ((GridView)sender).SelectedItem = null;
        }
    }
}
