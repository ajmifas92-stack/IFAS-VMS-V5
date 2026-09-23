# IFAS VMS — Full Development Project

This repository is the complete IFAS VMS development source package.

## Modules
1. `01-IFAS-VMS-SERVER` — ASP.NET Core server, users, cameras, licenses, audit.
2. `02-IFAS-LICENSE-MANAGER` — signed license creation/management.
3. `03-IFAS-VMS-CLIENT` — Windows WPF client with RTSP live view, ONVIF discovery, per-camera recording folders, FFmpeg recording/snapshot controls.
4. `04-IFAS-VMS-DATABASE` — logical schema, seeds and database documentation.
5. `05-IFAS-VMS-SHARED` — shared contracts and DTOs.
6. `06-IFAS-VMS-SECURITY` — reusable security/crypto helpers.
7. `07-IFAS-VMS-INSTALLER` — Windows installation/bootstrap scripts.
8. `08-IFAS-VMS-UPDATER` — update staging, verification, backup and rollback foundation.
9. `09-IFAS-VMS-DOCUMENTATION` — product and operational documentation.

## Current camera workflow
The Windows client now has actual camera I/O code rather than UI placeholders:

- Enter an RTSP URL and connect to live view.
- Discover ONVIF transmitters on the LAN using WS-Discovery.
- Select a recording directory independently for each camera.
- Start/stop FFmpeg recording into dated folders and 5-minute MP4 segments.
- Capture snapshots to a camera-specific `Snapshots` folder.
- Open the camera's data folder directly from the client.
- Persist camera configuration under `%ProgramData%\\IFAS-VMS\\client.json`.

## Important compatibility note
There is no universal RTSP URL that works for every camera brand/model. ONVIF discovery can find a compatible transmitter, but the exact media/RTSP endpoint and authentication behaviour can vary by vendor and firmware. The client therefore supports manual RTSP URLs as well as ONVIF discovery.

## Prerequisites on a Windows PC
- .NET 8 SDK.
- Internet access for the first NuGet restore/build.
- FFmpeg installed and available as `ffmpeg.exe` on PATH for recording/snapshots.
- An RTSP/ONVIF camera for real camera testing.

## Build
From the repository root:

`dotnet restore IFAS-VMS.sln`

`dotnet build IFAS-VMS.sln -c Release`

The environment used to assemble this source archive does not contain the .NET SDK, so a Windows build was not executed here. The repository is therefore a source/test package, not a claim of a precompiled production installer.

## Recommended test order
1. Build the solution on Windows.
2. Run the client with no camera and confirm configuration/storage UI.
3. Install/configure FFmpeg.
4. Add one RTSP camera and test live view.
5. Select a recording folder and test recording/segmentation.
6. Test snapshot capture.
7. Test ONVIF discovery on the same LAN.
8. Test camera disconnect/reconnect.
9. Test server authentication, roles and license limits.
10. Only after testing, package a signed installer/release.

## Security
Do not commit real camera passwords, private signing keys, production JWT secrets, or customer data to GitHub. Replace development secrets before deployment. Production camera credentials should be protected with Windows DPAPI/Credential Manager or another dedicated secret store.

## V2 additions
- Added `02-IFAS-LICENSE-MANAGER/IFAS.LicenseCreator.Desktop` as a supplier-only Windows License Creator UI.
- Expanded the WPF client into a dashboard-style shell with navigation, camera grid, camera list/PTZ panel, system overview, events and storage panels while retaining the existing camera I/O handlers.
