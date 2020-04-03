using System;
using System.Timers;
using Tizen;
using Tizen.Applications;
using Tizen.Applications.Notifications;


namespace NotificationSenderService
{
    class App : ServiceApplication
    {
        private bool isRunning = false;
        private Timer intervalTimer;
        private readonly String UiAppID = "org.tizen.example.NotificationTest";
        private readonly String ServiceAppID = "org.tizen.example.NotificationSenderService";

        void PostNotification()
        {
            DirectoryInfo info = Tizen.Applications.Application.Current.DirectoryInfo;
            string iconPath;
            string bgPath;
            string sharedPath = info.SharedResource;
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
            iconPath = sharedPath + "NotificationTestServiceApp.png";
            bgPath = sharedPath + "bg_square.jpg";

            Notification noti = new Notification
            {
                Title = "COVID-19 TEST",
                Content = customViewNotiBody,
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

            noti.AddStyle(style);
            Log.Info("VCApp", "[VC] Post service........");
            NotificationManager.Post(noti);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Console.WriteLine("[VC] OnCreate service........");
            intervalTimer = new Timer(10000);
            intervalTimer.Elapsed += new ElapsedEventHandler((s, e) => { Console.WriteLine("[VC] Service App........"); PostNotification(); });
            intervalTimer.AutoReset = true;
            intervalTimer.Enabled = false;
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            ReceivedAppControl receivedAppControl = e.ReceivedAppControl;

            try
            {
                string op = receivedAppControl.ExtraData.Get("op") as string;
                if (op != null)
                {
                    Console.WriteLine($"Receive From UI App Operation : {op}");
                    if (op.Equals("start"))
                    {
                        intervalTimer.Enabled = true;
                    }
                    else if (op.Equals("stop"))
                    {
                        Exit();
                    }
                }
            }
            catch
            {
                Console.WriteLine($"No operation");
            }

            base.OnAppControlReceived(e);
        }

        protected override void OnDeviceOrientationChanged(DeviceOrientationEventArgs e)
        {
            base.OnDeviceOrientationChanged(e);
        }

        protected override void OnLocaleChanged(LocaleChangedEventArgs e)
        {
            base.OnLocaleChanged(e);
        }

        protected override void OnLowBattery(LowBatteryEventArgs e)
        {
            base.OnLowBattery(e);
        }

        protected override void OnLowMemory(LowMemoryEventArgs e)
        {
            base.OnLowMemory(e);
        }

        protected override void OnRegionFormatChanged(RegionFormatChangedEventArgs e)
        {
            base.OnRegionFormatChanged(e);
        }

        protected override void OnTerminate()
        {
            Log.Info("VCApp", "[VC] OnTerminate service........");
            base.OnTerminate();
        }

        static void Main(string[] args)
        {
            App app = new App();
            app.Run(args);
        }
    }
}
