
using Plugin.BLE;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.Devices
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Devices : ContentView
    {
        public List<Plugin.BLE.Abstractions.Contracts.IDevice> deviceList = new();
        private Plugin.BLE.Abstractions.Contracts.IBluetoothLE ble;
        private Plugin.BLE.Abstractions.Contracts.IAdapter adapter;

        public Devices()
        {
            InitializeComponent();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            _ = CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

            adapter.DeviceDiscovered += (s, a) =>
            {
                deviceList.Add(a.Device);
                Debug.WriteLine("Device found  " + a.Device.Name);
            };

            deviceListView.BeginRefresh();
        }

        private async void DeviceListView_ItemSelected(object _, SelectedItemChangedEventArgs e)
        {
            var selectedDevice = (Plugin.BLE.Abstractions.Contracts.IDevice)e.SelectedItem;
            try
            {
                await adapter.ConnectToDeviceAsync(selectedDevice);
                Debug.WriteLine(selectedDevice.State);
            }
            catch (DeviceConnectionException m)
            {
                Debug.WriteLine(m.Message);
            }
           
        }

        private async void RefreshDeviceList(object _ = null, System.EventArgs e = null)
        {

            var systemDevices = adapter.GetSystemConnectedOrPairedDevices();
            deviceList.AddRange((List<Plugin.BLE.Abstractions.Contracts.IDevice>)systemDevices);

            adapter.ScanTimeout = 5000;
            await adapter.StartScanningForDevicesAsync();
            deviceListView.EndRefresh();
            deviceListView.ItemsSource = deviceList;
        }
    }
}