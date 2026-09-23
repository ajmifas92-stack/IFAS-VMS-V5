# License Management

License Manager creates and signs IFAS license files.

A license can contain limits such as:

- Maximum users
- Maximum cameras
- Expiry date
- Feature list
- Customer/license identifier

## Security model

The License Manager uses a private signing key.

The VMS Server uses the corresponding public key to verify the license.

The private signing key must never be placed in:
- VMS Client
- VMS Server
- Installer
- Public repository
- Customer-distributed package

## Example

If a license allows 10 users and 32 cameras:
- User 11 must be rejected by server-side licensing logic.
- Camera 33 must be rejected by server-side licensing logic.
