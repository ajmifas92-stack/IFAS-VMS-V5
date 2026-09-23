# Architecture

```text
                    +----------------------+
                    |   IFAS VMS Client    |
                    |      Windows         |
                    +----------+-----------+
                               |
                         HTTPS / API
                               |
                    +----------v-----------+
                    |     IFAS Server      |
                    | Auth / Users /       |
                    | Cameras / License    |
                    +----+------------+----+
                         |            |
                       DB          Cameras
                         |
                +--------v---------+
                | IFAS VMS Database|
                +------------------+

             +----------------------+
             | IFAS License Manager |
             | License issue/sign   |
             +----------------------+

             +----------------------+
             | IFAS VMS Updater     |
             +----------------------+
```

## Trust model

- License Manager keeps the private signing key.
- Server verifies licenses using the public key.
- Client communicates with the Server.
- License enforcement is server-side.
- Shared package contains common contracts.
