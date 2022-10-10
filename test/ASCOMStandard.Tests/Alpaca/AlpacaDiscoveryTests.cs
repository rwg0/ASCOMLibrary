﻿using ASCOM.Alpaca.Clients;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static ASCOM.Common.Devices;

namespace ASCOM.Alpaca.Tests.Alpaca
{

    /// <summary>
    /// This test class assumes that an Alpaca Telescope and Camera are accessible on the network
    /// </summary>
    public class AlpacaDiscoveryTests
    {
        #region Synchronous methods

        [Fact]
        public void AlpacaCamera()
        {
            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera));

            AlpacaCamera camera = AlpacaClient.GetDevice<AlpacaCamera>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera)[0], 100, 100, 100, 333, null, null, true, null);
            Assert.IsType<AlpacaCamera>(camera);
            camera.Dispose();
        }

        [Fact]
        public void AlpacaTelescope()
        {
            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Telescope));

            AlpacaTelescope telescope = AlpacaClient.GetDevice<AlpacaTelescope>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Telescope)[0], 100, 100, 100, 333, null, null, true, null);
            Assert.IsType<AlpacaTelescope>(telescope);
            telescope.Dispose();
        }

        [Fact]
        public void AlpacaBadAscomDevice()
        {
            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.Throws<InvalidValueException>(() => AlpacaClient.GetDevice<AlpacaCamera>(null, 100, 100, 100, 333, null, null, true, null));
        }

        [Fact]
        public void AlpacaCameraPropertiesNullValues()
        {
            const int CONNECTION_TIMEOUT = 7;
            const int SHORT_TIMEOUT = 13;
            const int LONG_TIMEOUT = 238;
            const uint CLIENT_NUMBER = 729;

            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera));

            AlpacaCamera camera = AlpacaClient.GetDevice<AlpacaCamera>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera)[0], CONNECTION_TIMEOUT, SHORT_TIMEOUT, LONG_TIMEOUT, CLIENT_NUMBER, null, null, true, null);

            Assert.Equal(CONNECTION_TIMEOUT, camera.ClientConfiguration.EstablishConnectionTimeout);
            Assert.Equal(SHORT_TIMEOUT, camera.ClientConfiguration.StandardDeviceResponseTimeout);
            Assert.Equal(LONG_TIMEOUT, camera.ClientConfiguration.LongDeviceResponseTimeout);
            Assert.Equal(CLIENT_NUMBER, camera.ClientConfiguration.ClientNumber);
            Assert.Equal(DeviceTypes.Camera, camera.ClientConfiguration.DeviceType);
            Assert.Equal(ImageArrayTransferType.BestAvailable, camera.ImageArrayTransferType);
            Assert.Equal(ImageArrayCompression.None, camera.ImageArrayCompression);
            Assert.Null(camera.ClientConfiguration.UserName);
            Assert.Null(camera.ClientConfiguration.Password);
            Assert.Null(camera.ClientConfiguration.Logger);

            camera.Dispose();
        }

        [Fact]
        public void AlpacaCameraPropertiesNotNullValues()
        {
            const int CONNECTION_TIMEOUT = 3;
            const int SHORT_TIMEOUT = 19;
            const int LONG_TIMEOUT = 426;
            const uint CLIENT_NUMBER = 10586;

            TraceLogger TL = new TraceLogger("TestLogger", false);
            const string USER_NAME = "asdwer52?";
            const string USER_PASSWORD = "$%Sg90|@!56BhI";

            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera));

            AlpacaCamera camera = AlpacaClient.GetDevice<AlpacaCamera>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera)[0], CONNECTION_TIMEOUT, SHORT_TIMEOUT, LONG_TIMEOUT, CLIENT_NUMBER, USER_NAME, USER_PASSWORD, true, TL);

            Assert.Equal(CONNECTION_TIMEOUT, camera.ClientConfiguration.EstablishConnectionTimeout);
            Assert.Equal(SHORT_TIMEOUT, camera.ClientConfiguration.StandardDeviceResponseTimeout);
            Assert.Equal(LONG_TIMEOUT, camera.ClientConfiguration.LongDeviceResponseTimeout);
            Assert.Equal(CLIENT_NUMBER, camera.ClientConfiguration.ClientNumber);
            Assert.Equal(DeviceTypes.Camera, camera.ClientConfiguration.DeviceType);
            Assert.Equal(ImageArrayTransferType.BestAvailable, camera.ImageArrayTransferType);
            Assert.Equal(ImageArrayCompression.None, camera.ImageArrayCompression);
            Assert.Equal(USER_NAME, camera.ClientConfiguration.UserName);
            Assert.Equal(USER_PASSWORD, camera.ClientConfiguration.Password);
            Assert.Same(TL, camera.ClientConfiguration.Logger);

            camera.Dispose();
        }

        [Fact]
        public void AlpacaCameraMinimalValues()
        {

            // Expected default client values
            const int CONNECTION_TIMEOUT = 3;
            const int SHORT_TIMEOUT = 3;
            const int LONG_TIMEOUT = 100;

            const string USER_NAME = "";
            const string USER_PASSWORD = "";

            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera));

            AlpacaCamera camera = AlpacaClient.GetDevice<AlpacaCamera>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera)[0]);

            Assert.Equal(CONNECTION_TIMEOUT, camera.ClientConfiguration.EstablishConnectionTimeout);
            Assert.Equal(SHORT_TIMEOUT, camera.ClientConfiguration.StandardDeviceResponseTimeout);
            Assert.Equal(LONG_TIMEOUT, camera.ClientConfiguration.LongDeviceResponseTimeout);
            Assert.True(camera.ClientConfiguration.ClientNumber > 0);
            Assert.Equal(DeviceTypes.Camera, camera.ClientConfiguration.DeviceType);
            Assert.Equal(ImageArrayTransferType.JSON, camera.ImageArrayTransferType);
            Assert.Equal(ImageArrayCompression.None, camera.ImageArrayCompression);
            Assert.Equal(USER_NAME, camera.ClientConfiguration.UserName);
            Assert.Equal(USER_PASSWORD, camera.ClientConfiguration.Password);
            Assert.Null(camera.ClientConfiguration.Logger);

            camera.Dispose();
        }

        [Fact]
        public void AlpacaTelescopeMinimalValues()
        {

            // Expected default client values
            const int CONNECTION_TIMEOUT = 3;
            const int SHORT_TIMEOUT = 3;
            const int LONG_TIMEOUT = 100;

            const string USER_NAME = "";
            const string USER_PASSWORD = "";

            AlpacaDiscovery alpacaDisocvery = new AlpacaDiscovery();
            alpacaDisocvery.StartDiscovery(1, 100, 32227, 1.0, false, true, false, ServiceType.Http);
            do
            {
                Thread.Sleep(50);
            } while (!alpacaDisocvery.DiscoveryComplete);

            Assert.NotEmpty(alpacaDisocvery.GetAscomDevices(DeviceTypes.Telescope));

            AlpacaTelescope telescope = AlpacaClient.GetDevice<AlpacaTelescope>(alpacaDisocvery.GetAscomDevices(DeviceTypes.Camera)[0]);

            Assert.Equal(CONNECTION_TIMEOUT, telescope.ClientConfiguration.EstablishConnectionTimeout);
            Assert.Equal(SHORT_TIMEOUT, telescope.ClientConfiguration.StandardDeviceResponseTimeout);
            Assert.Equal(LONG_TIMEOUT, telescope.ClientConfiguration.LongDeviceResponseTimeout);
            Assert.True(telescope.ClientConfiguration.ClientNumber > 0);
            Assert.Equal(DeviceTypes.Telescope, telescope.ClientConfiguration.DeviceType);
            Assert.Equal(USER_NAME, telescope.ClientConfiguration.UserName);
            Assert.Equal(USER_PASSWORD, telescope.ClientConfiguration.Password);
            Assert.Null(telescope.ClientConfiguration.Logger);

            telescope.Dispose();
        }

        #endregion

        #region AscomDevice filtering and ordering

        [Fact]
        public async void GetAscomDevicesSelectDeviceType()
        {
            TraceLogger TL = new TraceLogger("GetAscomDevicesSelectDeviceType", true);
            TL.LogMessage("Test", $"About to call GetAscomDevicesAsync");

            // Get every ASCOM device from all Alpaca discovered devices into a List
            List<AscomDevice> allAscomDevices = await AlpacaDiscovery.GetAscomDevicesAsync(null); // Or use a discovery instance and the GetAscomDevices method

            // Create a single device subset of Telescope devices (Could be achieved more simply by supplying a DeviceTypes parameter value in place of the null parameter in the command above)
            var ascomDevices = allAscomDevices.Where(info => info.AscomDeviceType == DeviceTypes.Telescope);
            TL.LogMessage("Test", $"Returned from GetAscomDevicesAsync");

            Assert.NotEmpty(ascomDevices);
            TL.LogMessage("Test", $"Found {ascomDevices.Count()} Telescope devices");
            foreach (AscomDevice ascomDevice in ascomDevices)
            {
                Assert.False(String.IsNullOrEmpty(ascomDevice.ServerName));
                Assert.False(String.IsNullOrEmpty(ascomDevice.Manufacturer));
                Assert.False(String.IsNullOrEmpty(ascomDevice.ManufacturerVersion));
                Assert.False(String.IsNullOrEmpty(ascomDevice.Location));
                TL.LogMessage("Test", $"Found {ascomDevice.ServerName} - {ascomDevice.Manufacturer} {ascomDevice.ManufacturerVersion} - Located at {ascomDevice.Location} - On IP Address: {ascomDevice.IpAddress}");
            }
        }

        [Fact]
        public async void GetAscomDevicesGroupBy()
        {
            TraceLogger TL = new TraceLogger("GetAscomDevicesGroupBy", true);
            TL.LogMessage("Test", $"About to call GetAscomDevicesAsync");

            // Get every ASCOM device from all Alpaca discovered devices into a List
            List<AscomDevice> ascomDevices = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.CoverCalibrator); // Or use a discovery instance and the GetAscomDevices method
            TL.LogMessage("Test", $"Returned from GetAscomDevicesAsync");

            var groupedDevices = ascomDevices.GroupBy(info => info.Location, info => info);

            Assert.NotEmpty(groupedDevices);
            TL.LogMessage("Test", $"Found {groupedDevices.Count()} CoverCalibrator locations");
            TL.BlankLine();
            foreach (var group in groupedDevices)
            {
                TL.LogMessage("Test", $"Group {group.Key}");
                Assert.False(String.IsNullOrEmpty(group.Key));
                Assert.True(group.Count<AscomDevice>() > 0);
                List<AscomDevice> ascomDevices1 = group.ToList<AscomDevice>();
                foreach (AscomDevice ascomDevice in ascomDevices1)
                {
                    Assert.False(String.IsNullOrEmpty(ascomDevice.ServerName));
                    Assert.False(String.IsNullOrEmpty(ascomDevice.Manufacturer));
                    Assert.False(String.IsNullOrEmpty(ascomDevice.ManufacturerVersion));
                    Assert.False(String.IsNullOrEmpty(ascomDevice.Location));
                    TL.LogMessage("Test", $"  Found {ascomDevice.ServerName} - {ascomDevice.Manufacturer} {ascomDevice.ManufacturerVersion} - Located at {ascomDevice.Location} - On IP Address: {ascomDevice.IpAddress}");

                }

            }
        }

        #endregion


        #region Async methods

        [Fact]
        public void GetAlpacaDevicesAsync()
        {
            TraceLogger TL = new TraceLogger("GetAlpacaDevicesAsync", true);
            TL.LogMessage("Test", $"About to call GetAlpacaDevicesAsync");

            List<AlpacaDevice> alpacaDevices = FetchAlpacaDevices(TL);
            Assert.NotEmpty(alpacaDevices);
            TL.LogMessage("Test", $"Returned from GetAlpacaDevicesAsync");
            TL.LogMessage("Test", $"Found {alpacaDevices.Count} camera devices");
            foreach (AlpacaDevice alpacaDevice in alpacaDevices)
            {
                TL.LogMessage("Test", $"Found {alpacaDevice.ServerName} - {alpacaDevice.Manufacturer} {alpacaDevice.ManufacturerVersion}");
                foreach (AscomDevice ascomDevice in alpacaDevice.AscomDevices(null))
                {
                    TL.LogMessage("Test", $"Found {ascomDevice.AscomDeviceType} device: {ascomDevice.AscomDeviceName} devices");
                }
            }
        }

        [Fact]
        public void GetAscomDevicesAsync()
        {
            TraceLogger TL = new TraceLogger("GetAscomDevicesAsync", true);
            TL.LogMessage("Test", $"About to call GetAscomDevices");

            List<AscomDevice> ascomDevices = FetchAscomDevices(DeviceTypes.Camera, TL);
            Assert.NotEmpty(ascomDevices);
            TL.LogMessage("Test", $"Returned from GetAscomDevices");
            TL.LogMessage("Test", $"Found: {ascomDevices[0].AscomDeviceName}");
            TL.LogMessage("Test", $"Found {ascomDevices.Count} camera devices");
            foreach (AscomDevice ascomDevice in ascomDevices)
            {
                TL.LogMessage("Test", $"Found {ascomDevice.AscomDeviceType} device: {ascomDevice.AscomDeviceName} devices");
            }
        }

        [Fact]
        public void GetAscomDevicesNullAsync()
        {
            TraceLogger TL = new TraceLogger("GetAscomDevicesNullAsync", true);
            TL.LogMessage("Test", $"About to call GetAscomDevices");

            List<AscomDevice> ascomDevices = FetchAscomDevices(null, TL);
            Assert.NotEmpty(ascomDevices);
            TL.LogMessage("Test", $"Returned from GetAscomDevices");
            TL.LogMessage("Test", $"Found {ascomDevices.Count} devices");
            foreach (AscomDevice ascomDevice in ascomDevices)
            {
                TL.LogMessage("Test", $"Found {ascomDevice.AscomDeviceType} device: {ascomDevice.AscomDeviceName} devices");
            }

        }

        [Fact]
        public void ConcurrentDiscoveriesAsync()
        {
            TraceLogger TL = new TraceLogger("ConcurrentDiscoveriesAsync", true);
            TL.LogMessage("Test", $"About to create async discovery methods");
            Task<List<AscomDevice>> focuserDevices = AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Focuser, 2, 100, 32227, 2.0, false, true, false, ServiceType.Http, TL);
            TL.LogMessage("Test", $"Created focuser devices task");

            Task<List<AscomDevice>> telescopeDevices = AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Telescope, 2, 100, 32227, 2.0, false, true, false, ServiceType.Http, TL);
            TL.LogMessage("Test", $"Created telescope devices task");

            Task<List<AscomDevice>> cameramDevices = AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Camera, 2, 100, 32227, 2.0, false, true, false, ServiceType.Http, TL);
            TL.LogMessage("Test", $"Created camera devices task");

            TL.LogMessage("Test", $"Waiting for tasks to complete...");
            Task.WaitAll(focuserDevices, telescopeDevices, cameramDevices);

            TL.LogMessage("Test", $"Tasks completed: {focuserDevices.Status}");

            Assert.Equal(TaskStatus.RanToCompletion, focuserDevices.Status);
            Assert.Equal(TaskStatus.RanToCompletion, telescopeDevices.Status);
            Assert.Equal(TaskStatus.RanToCompletion, cameramDevices.Status);

            Assert.NotEmpty(focuserDevices.Result);
            Assert.NotEmpty(telescopeDevices.Result);
            Assert.NotEmpty(cameramDevices.Result);

            if (focuserDevices.Status == TaskStatus.RanToCompletion)
            {
                TL.LogMessage("Test", $"Returned {focuserDevices.Result.Count} Focuser devices. Found: {focuserDevices.Result[0].AscomDeviceName}");
            }

            if (telescopeDevices.Status == TaskStatus.RanToCompletion)
            {
                TL.LogMessage("Test", $"Returned {telescopeDevices.Result.Count} Telescope devices. Found: {telescopeDevices.Result[0].AscomDeviceName}");
            }

            if (cameramDevices.Status == TaskStatus.RanToCompletion)
            {
                TL.LogMessage("Test", $"Returned {cameramDevices.Result.Count} Camera devices. Found: {cameramDevices.Result[0].AscomDeviceName}");
            }

        }

        #endregion 

        #region Support code
        static List<AscomDevice> FetchAscomDevices(DeviceTypes? deviceTypes, TraceLogger TL)
        {
            return AlpacaDiscovery.GetAscomDevicesAsync(deviceTypes, 1, 100, 32227, 4.0, false, true, false, ServiceType.Http, TL).Result;
        }

        static List<AlpacaDevice> FetchAlpacaDevices(TraceLogger TL)
        {
            return AlpacaDiscovery.GetAlpacaDevicesAsync(1, 100, 32227, 4.0, false, true, false, ServiceType.Http, TL).Result;
        }

        #endregion
    }
}