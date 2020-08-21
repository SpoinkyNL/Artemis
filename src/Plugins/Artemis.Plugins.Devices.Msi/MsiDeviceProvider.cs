﻿using System.IO;
using Artemis.Core.Plugins.DeviceProviders;
using Artemis.Core.Services.Interfaces;
using RGB.NET.Core;
using RGB.NET.Devices.Msi;

namespace Artemis.Plugins.Devices.Msi
{
    // ReSharper disable once UnusedMember.Global
    public class MsiDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public MsiDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Msi.MsiDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void EnablePlugin()
        {
            PathHelper.ResolvingAbsolutePath += (sender, args) => ResolveAbsolutePath(typeof(MsiRGBDevice<>), sender, args);
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(PluginInfo.Directory.FullName, "x64", "MysticLight_SDK.dll"));
            RGB.NET.Devices.Msi.MsiDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(PluginInfo.Directory.FullName, "x86", "MysticLight_SDK.dll"));
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }
    }
}