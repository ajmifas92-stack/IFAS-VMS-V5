# Installation Guide

## Recommended order

1. Prepare Windows x64 machine.
2. Install/publish IFAS VMS Server.
3. Configure database and HTTPS.
4. Install License Manager where license administration is performed.
5. Activate a valid license on the Server.
6. Install IFAS VMS Client.
7. Connect Client to the Server.
8. Create users and assign roles.
9. Add cameras.
10. Test live view, recording/playback and audit logging.

## Development

The project targets .NET 8. Build and test on a Windows PC with the .NET 8 SDK.

## Production

Use a signed MSI/MSIX or enterprise deployment package. Do not distribute private signing keys.
