using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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

            TextBoxWireGuardInterfaceName.Text = "WireguardP2S";
        }

        private string previewString;

        private void SetPreview(string value)
        {
            TextBoxPreviewConfig.Text = value;
            previewString = value;
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

            SetPreview(cfg);
        }

        private void ButtonBrowseServerConfig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "conf files (*.conf)|*.conf|All files (*.*)|*.*";
            var result = ofd.ShowDialog();
            if(result.HasValue && result.Value)
            {
                TextBoxServerConfigPath.Text = ofd.FileName;
                GetConfig();
            }
        }

        private void ButtonTransform_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "rsc files (*.rsc)|*.rsc|All files (*.*)|*.*";
            var result = sfd.ShowDialog();
            if(result.HasValue && result.Value) 
            {
                if(string.IsNullOrEmpty(TextBoxPreviewConfig.Text))
                {
                    return;
                }

                if(!Directory.Exists(Path.GetDirectoryName(sfd.FileName)))
                {
                    return;
                }

                File.WriteAllText(sfd.FileName, ParseCommands(previewString));
            }
        }

        private string ParseCommands(string input)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"/interface wireguard peers");
            sb.AppendLine("");

            string pattern = @"\[Peer\]\r\nPublicKey\s=\s([a-zA-Z0-9\+\=\/]+)\r\nPresharedKey\s=\s([a-zA-Z0-9\+\=\/]+)\r\nAllowedIPs\s=\s([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\/[0-9]{1,2})";

            var matches = Regex.Matches(input,
                pattern,
                RegexOptions.None);

            int i = 1;
            foreach(Match match in matches)
            {
                string address = match.Groups[3].Value;
                string presharedKey = match.Groups[2].Value;
                string publicKey = match.Groups[1].Value;

                sb.AppendLine($"# Peer {i}");
                sb.AppendLine($"add allowed-address={address} comment=\"peer {i}\" interface=\"{TextBoxWireGuardInterfaceName.Text}\" preshared-key=\"{presharedKey}\" public-key=\"{publicKey}\"");
                sb.AppendLine("");
                i = i + 1;
            }

            return sb.ToString();
        }
    }
}
