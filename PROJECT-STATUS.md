# Project status — camera-capable development build

The package contains the full source architecture plus a first working camera I/O path in the Windows client.

### Working source components
- RTSP live playback: implemented through LibVLCSharp.
- Per-camera recording folder: implemented.
- FFmpeg recording: implemented as an external process integration.
- Snapshot capture: implemented through FFmpeg.
- ONVIF WS-Discovery: implemented for LAN transmitter discovery.
- Persistent client configuration: implemented.
- Server-side user/license foundations: implemented.

### Requires real-PC verification
- NuGet restore and Windows compilation.
- Native LibVLC runtime packaging.
- FFmpeg availability and codec behaviour.
- RTSP compatibility for each camera model.
- ONVIF behaviour for each vendor/firmware.
- Long-running recording stability.
- Multi-camera load/performance.
- Production installer signing and secure credential storage.

This distinction is intentional: source code can implement a feature, but camera/vendor compatibility and production reliability must be validated on the target Windows PC and hardware.
