﻿using Acr.UserDialogs;
using NL.HNOGames.Domoticz.Models;
using NL.HNOGames.Domoticz.Resources;
using NL.HNOGames.Domoticz.Views.Dialog;
using System;
using System.Collections.Generic;
using NL.HNOGames.Domoticz.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Multilingual;
using System.Linq;

namespace NL.HNOGames.Domoticz.Views.Settings
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SettingsPage
   {
      private SelectMultipleBasePage<ScreenModel> _oEnableScreenPage;
      private readonly Command _goToMainScreen;
      public List<string> Languages = new List<string>
    {
       "Catalan",
       "Czech",
       "Danish",
       "Dutch",
       "English",
       "Finnish",
       "French",
       "German",
       "Hungarian",
       "Italian",
       "Kabyle",
       "Norwegian",
       "Polish",
       "Portuguese",
       "Russian",
       "Spanish",
       "Slovak",
       "Swedish",
       "Turkish",
       "Ukrainian",
       "Chinese Simplified",
    };

      public SettingsPage()
      {
         Init();
      }

      public SettingsPage(Command mainScreen)
      {
         _goToMainScreen = mainScreen;
         Init();
      }

      /// <summary>
      /// Init settings screen
      /// </summary>
      private void Init()
      {
         InitializeComponent();
         Title = AppResources.action_settings;

         PremiumScreenSetup();

         //Startup Settings
         txtStartScherm.Items.Clear();
         txtStartScherm.Items.Add(AppResources.title_dashboard);
         txtStartScherm.Items.Add(AppResources.title_switches);
         txtStartScherm.Items.Add(AppResources.title_scenes);
         txtStartScherm.Items.Add(AppResources.title_temperature);
         txtStartScherm.Items.Add(AppResources.title_weather);
         txtStartScherm.Items.Add(AppResources.title_utilities);
         txtStartScherm.SelectedIndex = App.AppSettings.StartupScreen;
         txtStartScherm.SelectedIndexChanged += (sender, args) =>
         {
            App.AppSettings.StartupScreen = txtStartScherm.SelectedIndex;
         };

         //Dashboard sort
         swNoSort.IsToggled = App.AppSettings.NoSort;
         lblSort.Text = App.AppSettings.NoSort
             ? AppResources.sort_dashboardLikeServer_on
             : AppResources.sort_dashboardLikeServer_off;
         swNoSort.Toggled += (sender, args) =>
         {
            App.AppSettings.NoSort = swNoSort.IsToggled;
            lblSort.Text = App.AppSettings.NoSort
                   ? AppResources.sort_dashboardLikeServer_on
                   : AppResources.sort_dashboardLikeServer_off;
         };

         //Multi Server Settings
         swEnableMultiServer.IsToggled = App.AppSettings.MultiServer;
         swEnableMultiServer.Toggled += (sender, args) =>
         {
            App.AppSettings.MultiServer = swEnableMultiServer.IsToggled;
         };

         //Talk Back
         swEnableTalkBack.IsToggled = App.AppSettings.TalkBackEnabled;
         swEnableTalkBack.Toggled += (sender, args) =>
         {
            App.AppSettings.TalkBackEnabled = swEnableTalkBack.IsToggled;
            if (swEnableTalkBack.IsToggled && !App.AppSettings.PremiumBought)
            {
               swEnableTalkBack.IsToggled = false;
               App.ShowToast(AppResources.category_talk_back + " " + AppResources.premium_feature);
            }
         };

         //DarkTheme
         swDarkTheme.IsToggled = App.AppSettings.DarkTheme;
         swDarkTheme.Toggled += (sender, args) =>
         {
            App.AppSettings.DarkTheme = swDarkTheme.IsToggled;
            if (swDarkTheme.IsToggled && !App.AppSettings.PremiumBought)
            {
               swDarkTheme.IsToggled = false;
               App.ShowToast(AppResources.category_theme + " " + AppResources.premium_feature);
            }
            SetTheme();
         };

         //Dashboard show switches
         swShowSwitch.IsToggled = App.AppSettings.ShowSwitches;
         lblShowSwitch.Text = App.AppSettings.ShowSwitches
             ? AppResources.switch_buttons_on
             : AppResources.switch_buttons_off;
         swShowSwitch.Toggled += (sender, args) =>
         {
            App.AppSettings.ShowSwitches = swShowSwitch.IsToggled;
            lblShowSwitch.Text = App.AppSettings.ShowSwitches
                   ? AppResources.switch_buttons_on
                   : AppResources.switch_buttons_off;
         };

         //Enable notifications
         swEnableNotifications.IsToggled = App.AppSettings.EnableNotifications;
         swEnableNotifications.Toggled += (sender, args) =>
         {
            App.AppSettings.EnableNotifications = swEnableNotifications.IsToggled;
            if (swEnableNotifications.IsToggled && !App.AppSettings.PremiumBought)
            {
               swEnableNotifications.IsToggled = false;
               App.ShowToast(AppResources.notification_screen_title + " " + AppResources.premium_feature);
            }
         };

         //Dashboard extra data
         swExtraData.IsToggled = App.AppSettings.ShowExtraData;
         lblExtraData.Text = App.AppSettings.ShowExtraData
             ? AppResources.show_extra_data_on
             : AppResources.show_extra_data_off;
         swExtraData.Toggled += (sender, args) =>
         {
            App.AppSettings.ShowExtraData = swExtraData.IsToggled;
            lblExtraData.Text = App.AppSettings.ShowExtraData
                   ? AppResources.show_extra_data_on
                   : AppResources.show_extra_data_off;
         };

         //Set current language
         pckrLanguages.ItemsSource = Languages;
         pckrLanguages.SelectedItem = CrossMultilingual.Current.CurrentCultureInfo.EnglishName;
         pckrLanguages.SelectedIndexChanged += pckrLanguages_SelectedIndexChanged;
      }

      /// <summary>
      /// Set the theme of the app (Dark or light)
      /// </summary>
      private static void SetTheme()
      {
         Type merge;
         if (App.AppSettings.DarkTheme)
         {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
               merge = (new Themes.DarkAndroid()).GetType();
            else
               merge = (new Themes.DarkiOS()).GetType();
         }
         else
            merge = (new Themes.Base()).GetType();
         Application.Current.Resources.MergedWith = merge;
      }

      private void PremiumScreenSetup()
      {
         if (App.AppSettings.PremiumBought)
         {
            lyPremium.IsVisible = false;
         }
         else
         {
            lyPremium.IsVisible = true;
         }
      }

      /// <summary>
      /// Save the enable screen selection
      /// </summary>
      private void ExecuteSaveEnableScreensCommand()
      {
         if (_oEnableScreenPage == null) return;
         App.AppSettings.EnabledScreens = _oEnableScreenPage.GetAllItems();
         _goToMainScreen.Execute(null);
      }

      /// <summary>
      /// Enable or Disable screens
      /// </summary>
      private async void btnEnableScreens_Clicked(object sender, EventArgs e)
      {
         var items = App.AppSettings.EnabledScreens ?? new List<ScreenModel>
            {
                new ScreenModel {ID = "Dashboard", Name = AppResources.title_dashboard, IsSelected = true},
                new ScreenModel {ID = "Switch", Name = AppResources.title_switches, IsSelected = true},
                new ScreenModel {ID = "Scene", Name = AppResources.title_scenes, IsSelected = true},
                new ScreenModel {ID = "Temperature", Name = AppResources.title_temperature, IsSelected = true},
                new ScreenModel {ID = "Weather", Name = AppResources.title_weather, IsSelected = true},
                new ScreenModel {ID = "Utilities", Name = AppResources.title_utilities, IsSelected = true}
            };

         _oEnableScreenPage =
             new SelectMultipleBasePage<ScreenModel>(items, new Command(ExecuteSaveEnableScreensCommand))
             {
                Title = AppResources.enable_items
             };
         await Navigation.PushAsync(_oEnableScreenPage);
      }

      private async void btnServerSetup_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new ServerSettingsPage());
      }

      private async void btnShowLog_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new ServerLogsPage());
      }

      private async void btnUserVars_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new UserVariablesPage());
      }

      private async void btnEvents_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new EventsPage());
      }

      private async void btnDebugInfo_Clicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new DebugInfoPage());
      }

      private async void btnQRCode_Clicked(object sender, EventArgs e)
      {
         if (App.AppSettings.PremiumBought)
            await Navigation.PushAsync(new QrCodeSettingsPage());
         else
            App.ShowToast(AppResources.qrcode + " " + AppResources.premium_feature);
      }

      private async void btnSpeechSettings_Clicked(object sender, EventArgs e)
      {
         if (App.AppSettings.PremiumBought)
            await Navigation.PushAsync(new SpeechSettingsPage());
         else
            App.ShowToast(AppResources.Speech + " " + AppResources.premium_feature);
      }

      private async void btnBuyPremium_Clicked(object sender, EventArgs e)
      {
         var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
         {
            Message =
                 "There are several features in the domoticz app that are locked until you buy the premium version\r\n- no ads!!\r\n-notification support\r\n- theming\r\n- talkback\r\n- qrcode scanning\r\n\r\n- and more features in the future",
            OkText = "Buy",
            CancelText = "Cancel"
         });

         if (!result)
            return;

         if (await InAppPurchaseHelper.PurchaseItem())
            App.ShowToast("Thanks for buying premium!!");

         PremiumScreenSetup();
      }

      private async void BtnRestore_OnClicked(object sender, EventArgs e)
      {
         if (await InAppPurchaseHelper.PremiumAccountPurchased())
         {
            App.ShowToast("Thanks for buying premium!!");
            PremiumScreenSetup();
         }
         else
         {
            App.ShowToast("Could not restore your premium account.");
         }
      }

      private async void BtnCameras_OnClicked(object sender, EventArgs e)
      {
         await Navigation.PushAsync(new CameraPage());
      }

      private async void btnGeofenceSettings_Clicked(object sender, EventArgs e)
      {
         if (App.AppSettings.PremiumBought)
            await Navigation.PushAsync(new GeofenceSettingsPage());
         else
            App.ShowToast(AppResources.Speech + " " + AppResources.premium_feature);
      }

      private void pckrLanguages_SelectedIndexChanged(object sender, EventArgs e)
      {
         ///Set the new language
         App.AppSettings.SpecifiedLanguage = pckrLanguages.SelectedItem.ToString();
         CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains(App.AppSettings.SpecifiedLanguage));
         AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
         App.ShowToast(AppResources.restart_required_msg);
      }
   }
}