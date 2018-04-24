using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace weather.Views
{
    public sealed partial class 天气Page : Page, INotifyPropertyChanged
    {
         Geoposition location;
        public 天气Page()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            
        }
        //页面打开时
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if(city.Text == "loading...")
                await Getdate();
            
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void  refresh_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Getdate();
        }
        private async  Task Getdate()
        {
            loading.IsIndeterminate = true;
            try
            {
                location = await getlocation.Location();
                RootObject myweather = await getweather.GetWeather(location.Coordinate.Point.Position.Latitude, location.Coordinate.Point.Position.Longitude);
                city.Text = myweather.HeWeather5[0].basic.city;
                temp.Text = myweather.HeWeather5[0].daily_forecast[0].tmp.min + "°C~" + myweather.HeWeather5[0].daily_forecast[0].tmp.max + "°C";
                string icon = String.Format("ms-appx:///Assets/icon/{0}.png", myweather.HeWeather5[0].daily_forecast[0].cond.code_d);
                icond.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));
                icon = String.Format("ms-appx:///Assets/icon/{0}.png", myweather.HeWeather5[0].daily_forecast[0].cond.code_n);
                iconn.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));
                zhuan.Text = "~";
                if(myweather.HeWeather5[0].daily_forecast[0].cond.txt_d == myweather.HeWeather5[0].daily_forecast[0].cond.txt_n)
                {
                    weather.Text = myweather.HeWeather5[0].daily_forecast[0].cond.txt_d;
                }
                else
                {
                    weather.Text = myweather.HeWeather5[0].daily_forecast[0].cond.txt_d + "转" + myweather.HeWeather5[0].daily_forecast[0].cond.txt_n;
                }
                wind.Text = myweather.HeWeather5[0].daily_forecast[0].wind.sc;
                sug.Text = myweather.HeWeather5[0].suggestion.air.txt + "\n\n" + myweather.HeWeather5[0].suggestion.comf.txt + "\n\n" + myweather.HeWeather5[0].suggestion.cw.txt + "\n\n" + myweather.HeWeather5[0].suggestion.drsg.txt + "\n\n" + myweather.HeWeather5[0].suggestion.flu.txt + "\n\n" + myweather.HeWeather5[0].suggestion.sport.txt;
                loading.IsIndeterminate = false;
                loading.Opacity = 0;
                string text = city.Text + "\n\n " + temp.Text + "\n\n " + weather.Text;
                var tilexml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
                var tileattributes = tilexml.GetElementsByTagName("text");
                tileattributes[0].AppendChild(tilexml.CreateTextNode(text));
                var tilenotification = new TileNotification(tilexml);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tilenotification);
            }
            catch(System.UnauthorizedAccessException)
            {
                await new MessageDialog("请打开定位且在设置中允许定位").ShowAsync();
            }
            
        }
    }
}
