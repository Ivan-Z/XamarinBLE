using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AxiomaticBLE {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
            Connect();
        }

        public async void Connect() {
            IBluetoothLE ble;
            IAdapter adapter;
            IDevice axiomatic = null;

            Boolean connected = false;

            ObservableCollection<IDevice> deviceList;

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();

            adapter.DeviceDiscovered += (s, a) => {
                deviceList.Add(a.Device);
            };

            await adapter.StartScanningForDevicesAsync();

            System.Diagnostics.Debug.WriteLine("We have {0} discovered devices", deviceList.Count);

            foreach (var device in deviceList) {
                if (device.Id.ToString() == "00000000-0000-0000-0000-cc78ab660195") {
                    System.Diagnostics.Debug.WriteLine("Found axiomatic");
                    axiomatic = device;
                }

            }

            if (axiomatic != null) {
                try {
                    await adapter.ConnectToDeviceAsync(axiomatic);
                    connected = true;
                }
                catch (DeviceConnectionException e) {
                    System.Diagnostics.Debug.WriteLine("Could not connect to device");
                }
            }

            if (connected) {
                var services = await axiomatic.GetServicesAsync();
                foreach (var service in services) {
                    System.Diagnostics.Debug.WriteLine("Service: " + service.Name + " ID: " + service.Id);
                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (var characteristic in characteristics) {
                        System.Diagnostics.Debug.WriteLine("Characterstic: " + characteristic.Name + " Properties: " + characteristic.Properties + " ID: " + characteristic.Id + " Value: " + characteristic.Value);
                        var descriptors = await characteristic.GetDescriptorsAsync();
                        foreach (var descriptor in descriptors) {
                            System.Diagnostics.Debug.WriteLine("Characterstic: " + characteristic.Name + " Properties: " + characteristic.Properties + " ID: " + characteristic.Id + " Value: " + characteristic.Value);
                        }
                    }

                }
            }


        }
    }


}
