using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UCCBCTToolBox.Models;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for DeactivateDialog.xaml
    /// </summary>
    public partial class DeactivateDialog : Window
    {
        private int _a, _b, _sum;

        public int Sum
        {
            get { return _sum; }
            set { _sum = value; }
        }
        public int NumberA => _a;
        public int NumberB => _b;

        public DeactivateDialog()
        {
            Random random = new();
            _a = random.Next(5, 50);
            _b = random.Next(5, 50);

            DataContext = this;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private async void DeactivateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Sum != NumberA + NumberB)
                {
                    MessageBox.Show("Wrong answer.");
                    DialogResult = false;
                    return;
                }
                Cursor = Cursors.Wait;
                DeactivateButton.IsEnabled = false;

                HttpClient client = new HttpClient();

                object? userid = Global.GetSavedValueFromRegistry("UserID");
                object? serial = Global.GetSavedValueFromRegistry("Serial");

                var dict = new Dictionary<string, string>
                {
                    { "id", userid == null ? string.Empty : (string)userid },
                    { "password", serial == null ? string.Empty : (string)serial }
                };

                var req = new HttpRequestMessage(HttpMethod.Post, Global.DeActivationURL) { Content = new FormUrlEncodedContent(dict) };
                var res = await client.SendAsync(req);

                if (res.IsSuccessStatusCode)
                {
                    var response = await res.Content.ReadAsStringAsync();

                    ApiResponse resp = JsonConvert.DeserializeObject<ApiResponse>(response);
                    MessageBox.Show(resp.Message);
                    if (resp.Type == "success")
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        DialogResult = false;
                    }
                }
                else
                {
                    var response = await res.Content.ReadAsStringAsync();
                    ApiResponse resp = JsonConvert.DeserializeObject<ApiResponse>(response);
                    MessageBox.Show(resp.Message, "Unsuccessful response received.");
                    DialogResult = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DialogResult = false;
            }
            finally
            {
                Cursor = Cursors.Arrow;
                DeactivateButton.IsEnabled = true;
            }
        }
    }
}
