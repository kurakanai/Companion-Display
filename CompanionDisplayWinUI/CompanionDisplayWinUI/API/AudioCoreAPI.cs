using CoreAudio;

namespace CompanionDisplayWinUI.API
{
    static class AudioCoreAPI
    {
        public static MMDeviceEnumerator DevEnum = new();
        public static MMDevice mmDevices = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        private static readonly MMDeviceCollection audioEndpointCollection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);
        public static MMDevice[] getAllEndpoints = [.. audioEndpointCollection];
    }
}
