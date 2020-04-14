﻿using AssetViewer.Data;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AssetViewer.Controls {

  public partial class ItemCard : UserControl, INotifyPropertyChanged {

    #region Properties

    public bool CanSwap {
      get { return (bool)GetValue(CanSwapProperty); }
      set { SetValue(CanSwapProperty, value); }
    }

    public TemplateAsset SelectedAsset {
      get { return (TemplateAsset)GetValue(SelectedAssetProperty); }
      set { SetValue(SelectedAssetProperty, value); }
    }

    public LinearGradientBrush RarityBrush {
      get {
        var selection = this.SelectedAsset?.RarityType ?? "Common";
        switch (selection) {
          case "Uncommon":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(65, 89, 41), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Rare":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(50, 60, 83), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Epic":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(90, 65, 89), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Legendary":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(98, 66, 46), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          default:
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(126, 128, 125), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
        }
      }
    }

    public String AllocationText {
      get {
        return AssetProvider.Descriptions[-106];
      }
    }

    public String ExpeditionText {
      get {
        return AssetProvider.Descriptions[-1220];
      }
    }

    public String TradeText {
      get {
        return AssetProvider.Descriptions[12725];
      }
    }

    public String HiringFeeText {
      get {
        return AssetProvider.Descriptions[21731];
      }
    }

    public String ItemSetText {
      get {
        return AssetProvider.Descriptions[-1221];
      }
    }

    public String ItemTrasmutable {
      get {
        return AssetProvider.Descriptions[113817];
      }
    }

    public String ProductionText {
      get {
        return AssetProvider.Descriptions[100006];
      }
    }

    public String ConsumptionText {
      get {
        return AssetProvider.Descriptions[100007];
      }
    }

    public String BuildCostsText {
      get {
        return AssetProvider.Descriptions[100008];
      }
    }

    public String MaintenanceText {
      get {
        return AssetProvider.Descriptions[100409];
      }
    }

    public String UpgradeCostsText {
      get {
        return AssetProvider.Descriptions[2001775];
      }
    }

    #endregion Properties

    #region Fields

    public static readonly DependencyProperty CanSwapProperty =
                                                    DependencyProperty.Register("CanSwap", typeof(bool), typeof(ItemCard), new PropertyMetadata(false));

    public static readonly DependencyProperty SelectedAssetProperty =
        DependencyProperty.Register("SelectedAsset", typeof(TemplateAsset), typeof(ItemCard), new PropertyMetadata(null, OnSelectedAssetChanged));

    #endregion Fields

    #region Constructors

    public ItemCard() {
      InitializeComponent();
      Loaded += ItemCard_Loaded;
      Unloaded += ItemCard_Unloaded;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private static void OnSelectedAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is ItemCard card) {
        card.RaisePropertyChanged(nameof(RarityBrush));
      }
    }

    private void ItemCard_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
    }

    private void ItemCard_Loaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      mainGrid.DataContext = null;
      mainGrid.DataContext = this;
    }

    private void ButtonSwitch_Click(Object sender, RoutedEventArgs e) {
      if (this.ItemFront.Visibility == Visibility.Visible) {
        this.ItemFront.Visibility = Visibility.Collapsed;
        this.ItemBack.Visibility = Visibility.Visible;
      }
      else {
        this.ItemBack.Visibility = Visibility.Collapsed;
        this.ItemFront.Visibility = Visibility.Visible;
      }
    }

    #endregion Methods
  }
}