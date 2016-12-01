using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DataComparer.Common;
using WPFLibrary.Extensions;

namespace DataComparer.DesktopClient.Converters
{
    public class CellBackgroundColorConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Count() == 3)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
                    return Binding.DoNothing;


                string columnName = values[0].ConvertToString();
                DataRowView dataRow = values[1] as DataRowView;
                DataTable dataTable = values[2] as DataTable;
                if (columnName != null && dataRow != null && columnName != Constants.DatabaseTypeColumnName)
                {
                    var isColumnPresent = DynamicColumn.GetIsColumnPresentColumnName(columnName);
                    if (!dataRow[isColumnPresent].ConvertToBool())
                    {
                        return Brushes.Orange;
                    }

                    var isEqualColumnName = DynamicColumn.GetIsDataEqualColumnName(columnName);
                    if (!dataRow[isEqualColumnName].ConvertToBool())
                    {
                        return Brushes.Red;
                    }
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
