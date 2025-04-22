using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Pet_Adoption_WebAPI_Client.Converters
{
    public class ByteToStringConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return string.Empty;

                return String.Format("{0:n0}", value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string val = value.ToString();
                //strip out any characters that are not a digit
                string cleanVal = new string(val.Where(c => (char.IsDigit(c))).ToArray());
                return System.Convert.ToByte(cleanVal);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
