# Windows Service Deployment

The current bootstrap installer does not register the Server as a Windows Service.

For production:
1. Publish the server for win-x64.
2. Configure HTTPS and certificates.
3. Store secrets outside the installer.
4. Register the server using a signed service installer or deployment system.
5. Configure recovery/restart policy.
6. Restrict the service account permissions.
