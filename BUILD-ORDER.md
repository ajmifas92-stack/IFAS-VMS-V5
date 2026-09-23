# Build order

1. Restore NuGet packages.
2. Build `05-IFAS-VMS-SHARED` and `06-IFAS-VMS-SECURITY`.
3. Build `01-IFAS-VMS-SERVER`.
4. Build `02-IFAS-LICENSE-MANAGER`.
5. Build `03-IFAS-VMS-CLIENT`.
6. Install FFmpeg on the test PC for recording/snapshot functions.
7. Run camera integration tests with a real ONVIF/RTSP camera.
8. Run server/license tests.
9. Publish and package with module `07` only after the above tests pass.
