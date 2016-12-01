using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VFPCodeConverter.Common;

namespace VFPCodeConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            var codeConverter = new CodeConverter();
            codeConverter.Initialize();

            ConvertedCodeTextBox.Text = codeConverter.Convert(SourceCodeTextBox.Text);

            LogTextBox.Text = Logger.GetLog();
        }

        private void SourceCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SourceCodeTextBox.Text != null)
            {
                SourceCodeTextBox.Text = SourceCodeTextBox.Text.Replace(";\r\n", "");
            }
        }
    }
}
