﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AuditApp.Android;
using PushNotification.Plugin;
using NL.HNOGames.Domoticz.Helpers;

namespace NL.HNOGames.Domoticz.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        public static Context AppContext;

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            AppContext = this.ApplicationContext;

            //RegisterActivityLifecycleCallbacks(this);

            AndroidPlaystoreAudit.Instance.UsesUntilPrompt = 20;
            AndroidPlaystoreAudit.Instance.TimeUntilPrompt = new TimeSpan(0, 10, 0);

            CrossPushNotification.Initialize<CrossPushNotificationListener>("705545900899");
            CrossPushNotification.Current.Register();

            StartPushService();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            //UnregisterActivityLifecycleCallbacks(this);
        }

        public static void StartPushService()
        {
            AppContext.StartService(new Intent(AppContext, typeof(PushNotificationService)));

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(PushNotificationService)), 0);
                AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
                alarm.Cancel(pintent);
            }
        }

        public static void StopPushService()
        {
            AppContext.StopService(new Intent(AppContext, typeof(PushNotificationService)));
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(PushNotificationService)), 0);
                AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
                alarm.Cancel(pintent);
            }
        }
    }
}