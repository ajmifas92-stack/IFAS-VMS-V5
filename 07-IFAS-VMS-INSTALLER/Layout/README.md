# Package Layout

Before running Install.ps1, place published applications in:

Published/
  Server/
  Client/
  LicenseManager/

Optional files:

Config/
Keys/

The installer copies these into the configured Program Files and ProgramData locations.
Do not place production private signing keys in a distributable installer package.
