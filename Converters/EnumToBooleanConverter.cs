using System;
using System.Globalization;
using System.Windows.Data;

namespace GenshinMacro.Converters;

/// <summary>
/// Converts enum value + parameter to bool for RadioButton IsChecked binding.
/// Usage: IsChecked="{Binding SelectedSection, Converter={StaticResource EnumToBoolean}, ConverterParameter=AutoRotation}"
/// </summary>
public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is not string paramStr)
            return false;

        var enumName = Enum.GetName(value.GetType(), value);
        return string.Equals(enumName, paramStr, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true && parameter is string paramStr)
        {
            if (Enum.TryParse(targetType, paramStr, ignoreCase: true, out var result))
                return result!;
        }
        return Binding.DoNothing;
    }
}
