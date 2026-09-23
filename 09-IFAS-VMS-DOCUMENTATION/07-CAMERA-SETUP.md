# Camera Setup

The system is designed around IP cameras and common streaming protocols.

Typical fields:

- Camera name
- IP/hostname
- Port
- Protocol
- Stream URL
- Username
- Password
- Enabled/disabled status

## Security

Camera credentials should not be logged.

Production systems should encrypt stored camera credentials using a protected key.

## Testing

For each camera verify:
- Network reachability
- Authentication
- Stream availability
- Correct resolution
- Expected frame rate
- Recording behavior
