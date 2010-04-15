using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blokus.Logic;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Data;

namespace Blokus.Misc
{
    // Base class

    public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter
    where T : class, new()
    {
        private static T m_converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (m_converter == null)
            {
                m_converter = new T();
            }
            return m_converter;
        }

        #region IValueConverter Members

        public abstract object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture);


        public abstract object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture);

        #endregion

    }

    //Sample derived class

    public class PlayerToBrushConverter : ConverterMarkupExtension<PlayerToBrushConverter>
    {
        public override object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            Brush brush = null;
            if (value != null && value is Player)
            {               
                switch ((Player)value)
                {
                    case Player.Orange: brush = Brushes.Orange; break;
                    case Player.Violet: brush = Brushes.Violet; break;
                }
            }
            return brush;
        }

        public override object ConvertBack(object value,
            Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
