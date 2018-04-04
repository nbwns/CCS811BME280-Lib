using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace CCS811_BME280_Library
{
    public class SensorHelper
    {
        public enum Type
        {
            CCS811,
            CCS811BME280
        }

        public struct CO2TVOC
        {
            public int CO2;
            public int TVOC;
            public CO2TVOC(int co2, int tvoc)
            {
                CO2 = co2;
                TVOC = tvoc;
            }
        }

        private I2cDevice _i2CDevice;

        public CCS811BME280Sensor Sensor { get; private set; }

        public SensorHelper(Type sensorType)
        {
            Initialize(sensorType);
        }

        private async void Initialize(Type sensorType)
        {
            await InitializeI2C(sensorType);
            Sensor = new CCS811BME280Sensor(ref _i2CDevice);
            Sensor.Initialize();
        }

        private async Task InitializeI2C(Type sensorType)
        {
            var addr = CCS811BME280Sensor.CCS811_I2C_ADDR;
            if (sensorType == Type.CCS811BME280)
            {
                addr = CCS811BME280Sensor.CCS811BME280_I2C_ADDR;
            }

            var settings = new I2cConnectionSettings(addr);
            settings.BusSpeed = I2cBusSpeed.StandardMode;
            settings.SharingMode = I2cSharingMode.Shared;

            var devices = await DeviceInformation.FindAllAsync(I2cDevice.GetDeviceSelector("I2C1"));

            if (devices.Count == 0)
            {
                throw new Exception(sensorType.ToString() + " device not found");
            }

            _i2CDevice = await I2cDevice.FromIdAsync(devices[0].Id, settings);
        }

        public CO2TVOC ReadCO2TVOC()
        {
           var data = Sensor.ReadCO2TVOC();
            return new CO2TVOC(data[0], data[1]);
        }

    }
}
