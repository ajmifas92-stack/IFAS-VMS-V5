# API Contract Notes

The Shared module is the source for common DTOs and constants.

When changing an API:
1. Update the shared DTO/contract.
2. Update Server.
3. Update Client.
4. Update License Manager if applicable.
5. Update documentation.
6. Test backward compatibility.
7. Increment version when the contract change is breaking.

Avoid silently changing field meanings.
