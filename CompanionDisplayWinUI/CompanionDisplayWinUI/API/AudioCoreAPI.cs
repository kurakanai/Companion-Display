using CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI.API
{
    static class AudioCoreAPI
    {
        public static MMDeviceEnumerator DevEnum = new();
        public static MMDevice mmDevices = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        private static MMDeviceCollection audioEndpointCollection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);
        public static MMDevice[] getAllEndpoints = audioEndpointCollection.ToArray();
    }
}
