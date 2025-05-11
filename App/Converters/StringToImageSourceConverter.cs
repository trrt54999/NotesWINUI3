using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace App.Converters;

public class StringToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // converts a string into a BitmapImage object that can be used as a source for an Image element in XAML.
        if (value is string path && !string.IsNullOrEmpty(path))
        {
            try
            {
                return new BitmapImage(new Uri(path));
            }
            catch
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/NotesDefaultLogo.png"));
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
