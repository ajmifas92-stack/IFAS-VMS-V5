# IFAS License Manager - Keys

This folder contains the RSA key pair used to digitally sign IFAS license payloads.

Files created automatically:

- `ifas-license-private.pem`
- `ifas-license-public.pem`

IMPORTANT:

The private key is SECRET.

Never:
- upload the private key to a customer machine
- send the private key to customers
- publish the private key
- commit the private key to a public Git repository

The IFAS VMS Server only needs the PUBLIC key to verify licenses.

The current PEM-file storage is suitable for development. For production, move the private key to protected secure storage or a dedicated key-management system.
