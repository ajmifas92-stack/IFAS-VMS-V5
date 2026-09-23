# Key Management

## Development
Development keys may be generated using `RsaKeyGenerator`.

## Production
- Keep the License Manager private signing key outside source control.
- Prefer OS secret stores, HSM/KMS or an equivalent managed secret system.
- Only the public verification key should be distributed to the VMS Server.
- Rotate secrets according to an operational key-management policy.
- Never place private keys in Git, client installers or public downloads.

## JWT
Use a long random secret stored outside `appsettings.json` in production.
