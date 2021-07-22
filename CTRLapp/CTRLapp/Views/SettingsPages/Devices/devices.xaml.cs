
using Plugin.BLE;
using Plugin.BLE.Abstractions.Exceptions;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.Devices
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Devices : ContentView
    {
        public List<Plugin.BLE.Abstractions.Contracts.IDevice> deviceList = new List<Plugin.BLE.Abstractions.Contracts.IDevice>();
        private Plugin.BLE.Abstractions.Contracts.IBluetoothLE ble;
        private Plugin.BLE.Abstractions.Contracts.IAdapter adapter;

        public Devices()
        {
            InitializeComponent();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            adapter.DeviceDiscovered += (s, a) =>
            {
                deviceList.Add(a.Device);
                Debug.WriteLine("Device found" + a.Device.Name);
            };

            ble.StateChanged += (s, e) =>
            {
                Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
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
            Debug.WriteLine(ble.State);

            var systemDevices = adapter.GetSystemConnectedOrPairedDevices();
            deviceList.AddRange((List<Plugin.BLE.Abstractions.Contracts.IDevice>)systemDevices);

            adapter.ScanTimeout = 10000;
            await adapter.StartScanningForDevicesAsync();
            deviceListView.EndRefresh();
            deviceListView.ItemsSource = deviceList;
        }
    }
}