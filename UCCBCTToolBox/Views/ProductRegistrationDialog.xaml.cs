using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UCCBCTToolBox.Models;
using UCCBCTToolBox.ViewModels;

namespace UCCBCTToolBox.Views
{
    /// <summary>
    /// Interaction logic for ProductRegistrationDialog.xaml
    /// </summary>
    public partial class ProductRegistrationDialog : Window
    {
        private readonly ProductRegistrationViewModel model;
        public ProductRegistrationDialog()
        {
            InitializeComponent();
            model = (ProductRegistrationViewModel)DataContext;
            LoadSavedValues();
        }

        private bool LoadSavedValues()
        {
            object? installed = Global.GetSavedValueFromRegistry("Installed");
            if (installed == null)
            {
                installed = Global.SetKeyValueToRegistry("Installed", DateTime.Now);
            }
            if (installed != null)
            {

                model.TrialEnd = DateTime.Parse((string)installed).AddDays(7);
            }
            model.Serial = (string)Properties.Settings.Default["Serial"] ?? model.Serial;
            if (model.IsTrialEnd)
            {
                TrialButton.IsEnabled = false;
            }
            if (model.IsLicenseValid)
            {
                new MainWindow().Show();
                Close();
                return true;
            }
            return false;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SubmitButton.IsEnabled = false;

            if(LoadSavedValues())
            {
                return;
            }
            HttpClient client = new HttpClient();

            var dict = new Dictionary<string, string>
            {
                { "password", model.Serial.ToString() },
                { "device-name", System.Net.Dns.GetHostName() },
                { "device-hash", Global.GetMachineGuid() }
            };
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, Global.ActivationURL) { Content = new FormUrlEncodedContent(dict) };
                var res = await client.SendAsync(req);

                if (res.IsSuccessStatusCode)
                {
                    var response = await res.Content.ReadAsStringAsync();

                    ApiResponse resp = JsonConvert.DeserializeObject<ApiResponse>(response);
                    System.Windows.Forms.MessageBox.Show(resp.Message);
                    if (resp.Type == "success")
                    {
                        Global.SetKeyValueToRegistry("Serial", model.Serial);
                    }
                    else
                    {
                    }
                }

                if (model.IsLicenseValid)
                {
                    new MainWindow().Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                SubmitButton.IsEnabled = true;
            }
        }

        private void TrialButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosed(e);
        }
    }
}
