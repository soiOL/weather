using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace weather
{
    public class getlocation
    {
        public async static Task<Geoposition> Location()
        {
            //创建一个geolocator对象
            Geolocator geolocator = new Geolocator();
            //获取当前的地理位置信息
            Geoposition geoposition = await geolocator.GetGeopositionAsync();
            return geoposition;
        }
    }
}
