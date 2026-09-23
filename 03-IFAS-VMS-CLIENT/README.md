# IFAS VMS Client — working camera test build

This Windows WPF client contains the first real camera I/O layer for the IFAS VMS project.

## Implemented
- RTSP live view through LibVLCSharp/LibVLC.
- Camera profiles saved to `%ProgramData%\\IFAS-VMS\\client.json`.
- Per-camera recording folder selection.
- FFmpeg-based RTSP recording in 5-minute segments.
- FFmpeg snapshot capture.
- ONVIF WS-Discovery probe.
- Open recording/data folder from the UI.

## External runtime dependency
FFmpeg is required for recording and snapshots. Install a trusted Windows FFmpeg build and put `ffmpeg.exe` on PATH, or place it beside the client executable in a future signed installer package.

LibVLC native binaries are supplied by the `VideoLAN.LibVLC.Windows` NuGet package at build/publish time.

## Build
Run from the repository root:

`dotnet restore IFAS-VMS.sln`

`dotnet build IFAS-VMS.sln -c Release`

The camera client can then be run from the generated `03-IFAS-VMS-CLIENT/bin/Release/net8.0-windows/` output.

## Compatibility
RTSP URL formats vary by camera vendor. ONVIF discovery only discovers transmitters; it does not guarantee that a vendor exposes a universal RTSP path. For cameras that do not expose a usable ONVIF media profile, enter the vendor's RTSP URL manually.
