using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.Applications.Notifications;
using Tizen.Applications;

namespace NotificationTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CirclePage
    {
        private readonly String UiAppID = "org.tizen.example.NotificationTest";
        private readonly String ServiceAppID = "org.tizen.example.NotificationSenderService";
        static List<string> items = new List<string>
        {
            "My text 1", "My text 2", "Simple", "Service On", "Service Off"
        };

        public MainPage()
        {
            InitializeComponent();
            Console.WriteLine("VCApp UI App...");
            CreateNotificationTemplate(items[0], "bg.jpg", items[0], false);
            CreateNotificationTemplate(items[1], "bg_square.jpg", items[1], true);
        }

        public void onTestItemTapped(object sender, ItemTappedEventArgs args)
        {
        }

        public void CreateNotificationTemplate(String name, String bgImage, String title, bool isThreeButton)
        {
            DirectoryInfo info = Tizen.Applications.Application.Current.DirectoryInfo;
            String iconPath;
            String bgPath;
            String sharedPath = info.SharedResource;
            string customViewNotiBody =
                "{"
                + "\"template\": \"standard\","
                + "\"manifest_version\": 1,"
                + "\"model_info\": \"tizen_public\","
                + "\"category\": 3,"
                + "\"paragraph\": [{"
                + "\"type\": \"image\","
                + "\"content\": \"\","
                + "\"thumbnail\": \"/opt/usr/globalapps/org.tizen.example.NotificaiontTest/shared/res/bg_square.jpg\""
                + "}, {"
                + "\"type\": \"text\","
                + "\"content\": \"<color=#62dd16ff font_size=70 font=Tizen:style=Bold>2000</color><br/><color=#62dd16ff font_size=28 font=Tizen:style=Regular>/2000 steps</color>\""
                + "}, {"
                + "\"type\": \"text\","
                + "\"content\": \"<color=#ffffffff font_size=30 font=Tizen:style=Regular>You've earned step rewards once this week!</color>\""
                + "}],"
                + "\"always_wear\": 1"
                + "}";

            iconPath = sharedPath + "NotificaiontTest.png";
            bgPath = sharedPath + bgImage;

            Console.WriteLine($"ICON {bgPath}");
            Notification noti = new Notification
            {
                Title = title,
                Content = customViewNotiBody, // "quick brown fox jumps over the lazy dog",
                Icon = iconPath
            };

            Notification.AccessorySet accessory = new Notification.AccessorySet
            {
                SoundOption = AccessoryOption.On,
                CanVibrate = true
            };

            noti.Accessory = accessory;

            Notification.ActiveStyle style = new Notification.ActiveStyle
            {
                IsAutoRemove = true,
                BackgroundImage = bgPath
            };

            AppControl firstAppControl = new AppControl
            {
                ApplicationId = UiAppID
            };

            Notification.ButtonAction button1 = new Notification.ButtonAction
            {
                Index = Tizen.Applications.Notifications.ButtonIndex.First,
                Text = "Launch UI App",
                Action = firstAppControl
            };

            AppControl secondAppControl = new AppControl
            {
                ApplicationId = ServiceAppID
            };
            secondAppControl.ExtraData.Add("op", "stop");

            Notification.ButtonAction button2 = new Notification.ButtonAction
            {
                Index = Tizen.Applications.Notifications.ButtonIndex.Second,
                Text = "Stop Service Noti Loop",
                Action = secondAppControl
            };

            style.AddButtonAction(button1);
            style.AddButtonAction(button2);

            if (isThreeButton)
            {
                Notification.ButtonAction button3 = new Notification.ButtonAction
                {
                    Index = Tizen.Applications.Notifications.ButtonIndex.Third,
                    Text = "Btn-3",
                };
                style.AddButtonAction(button3);
            }

            noti.AddStyle(style);

            NotificationManager.SaveTemplate(noti, name);
        }

        public void PostNotificationWithTemplate(String templateName)
        {
            Notification loadNotification = NotificationManager.LoadTemplate(templateName);
            NotificationManager.Post(loadNotification);
        }

        public void PostNotification()
        {
            Console.WriteLine("PostNotification");

            DirectoryInfo info = Tizen.Applications.Application.Current.DirectoryInfo;
            String imagePath;
            String sharedPath = info.SharedResource;

            imagePath = sharedPath + "NotificaiontTest.png";

            Notification notification = new Notification
            {
                Title = "Notification TEST",
                Content = "Hello Tizen",
                Icon = imagePath
            };

            Notification.AccessorySet accessory = new Notification.AccessorySet
            {
                SoundOption = AccessoryOption.Custom,
                SoundPath = "Sound File Path",
                CanVibrate = true,
                LedOption = AccessoryOption.Custom,
                LedOnMillisecond = 100,
                LedOffMillisecond = 50,
                LedColor = Tizen.Common.Color.Lime
            };

            notification.Accessory = accessory;

            NotificationManager.Post(notification);
        }
        private void TwoButtonClicked(object sender, EventArgs e)
        {
            PostNotificationWithTemplate(items[0]);
        }

        private void ThreeButtonClicked(object sender, EventArgs e)
        {
            PostNotificationWithTemplate(items[1]);
        }

        private void SimpleClicked(object sender, EventArgs e)
        {
            PostNotification();
        }

        private void StartServiceClicked(object sender, EventArgs e)
        {
            Console.WriteLine("[VC] service start requested");
            AppControl appcontrol = new AppControl();

            appcontrol.Operation = AppControlOperations.Default;
            appcontrol.ApplicationId = ServiceAppID;

            var operation = "start";
            appcontrol.ExtraData.Add("op", operation);

            AppControl.SendLaunchRequest(appcontrol);
        }

        private void StopServiceClicked(object sender, EventArgs e)
        {
            Console.WriteLine("[VC] service stop requested");
            AppControl appcontrol = new AppControl();

            appcontrol.Operation = AppControlOperations.Default;
            appcontrol.ApplicationId = ServiceAppID;

            var operation = "stop";
            appcontrol.ExtraData.Add("op", operation);

            AppControl.SendLaunchRequest(appcontrol);
        }
    }
}