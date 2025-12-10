using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using UCCBCTToolBox.Classes;

namespace UCCBCTToolBox
{
    static class Global
    {
        private static Tool _tool = new Tool(null);
        public static Tool CurrentTool
        {
            get { return _tool; }
            set { _tool = value; }
        }
        private static string RegSubKey = "Software\\UCTool";
        private static string _domainAPI = "https://toolkit.advoaccessories.com/api/v1";
        public static string ActivationURL =  _domainAPI + "/activate";
        public static string DeActivationURL = _domainAPI + "/deactivate";
        public static object? SetKeyValueToRegistry(string valueKey, object value)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key != null)
                {
                    key.SetValue(valueKey, value);
                    return value;
                }
                return null;
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                MessageBox.Show("Details: " + ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static object? GetSavedValueFromRegistry(string valueKey)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key != null)
                {
                    return key.GetValue(valueKey);
                }
                else
                {
                    Registry.CurrentUser.CreateSubKey(RegSubKey);
                    return GetSavedValueFromRegistry(valueKey);
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                MessageBox.Show("Details: " + ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static bool DeleteValueFromRegistry(string valueKey)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key != null)
                {
                    key.DeleteValue(valueKey);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                MessageBox.Show("Details: " + ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static string GetMachineGuid()
        {
            try
            {
                string location = @"SOFTWARE\Microsoft\Cryptography";
                string name = "MachineGuid";

                using RegistryKey localMachineX64View =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using RegistryKey rk = localMachineX64View.OpenSubKey(location);
                if (rk == null)
                    throw new KeyNotFoundException(
                        string.Format("Key Not Found: {0}", location));

                object machineGuid = rk.GetValue(name);
                if (machineGuid == null)
                    throw new IndexOutOfRangeException(
                        string.Format("Index Not Found: {0}", name));

                return machineGuid.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return string.Empty;
            }
        }
    }
}
