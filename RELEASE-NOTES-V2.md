# IFAS VMS Complete Product V2 – Release Notes

This package consolidates the current IFAS VMS source tree and adds a dedicated supplier-side WPF License Creator plus a richer IFAS VMS client dashboard shell.

## Included
- IFAS VMS Server source
- Supplier-only IFAS License Manager API/source
- Supplier-only WPF IFAS License Creator
- Windows WPF VMS Client with RTSP/LibVLC camera I/O foundation
- ONVIF discovery foundation
- FFmpeg recording/snapshot foundation
- Database schema/reference package
- Shared models/DTOs
- Security library
- Installer/updater scripts and documentation
- User manuals and product documentation

## Important test status
The assembly environment does not contain the .NET 8 SDK or Windows runtime, so this source package was not compiled or hardware-tested here. Build it on a Windows PC with the .NET 8 SDK and then test with the target cameras, FFmpeg, network, storage and Windows services.

The visual dashboard contains real WPF controls and the existing camera action handlers, but it is not a claim that every commercial VMS feature (full multi-camera simultaneous decode, complete ONVIF media/PTZ, timeline playback, online activation backend, signed MSI, etc.) is already production-complete.
