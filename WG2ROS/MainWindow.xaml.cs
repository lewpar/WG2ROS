using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WG2ROS
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

        private void TextBoxServerConfigPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = TextBoxServerConfigPath.Text;

            LabelBrowsePlaceholder.Visibility = string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBoxServerConfigPath_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBoxServerConfigPath_PreviewDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                TextBoxServerConfigPath.Text = paths[0];
                GetConfig();
            }
        }

        private void GetConfig()
        {
            if(!File.Exists(TextBoxServerConfigPath.Text))
            {
                return;
            }

            var cfg = File.ReadAllText(TextBoxServerConfigPath.Text);

            TextBoxPreviewConfig.Text = cfg;
        }

        private void ButtonBrowseServerConfig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if(result.HasValue && result.Value)
            {
                TextBoxServerConfigPath.Text = ofd.FileName;
                GetConfig();
            }
        }
    }
}
