-- IFAS VMS reference seed data

INSERT OR IGNORE INTO Roles (Name, Description, IsActive)
VALUES
('SuperAdmin', 'Full system administration', 1),
('Administrator', 'System administration', 1),
('Operator', 'Camera and monitoring operations', 1),
('Viewer', 'View-only access', 1);

INSERT OR IGNORE INTO Permissions (Code, Name, Description, IsActive)
VALUES
('SYSTEM_VIEW', 'View System', 'View system information', 1),
('USER_VIEW', 'View Users', 'View users', 1),
('USER_CREATE', 'Create Users', 'Create users', 1),
('USER_UPDATE', 'Update Users', 'Update users', 1),
('USER_DELETE', 'Delete Users', 'Deactivate users', 1),
('CAMERA_VIEW', 'View Cameras', 'View cameras', 1),
('CAMERA_CREATE', 'Create Cameras', 'Create cameras', 1),
('CAMERA_UPDATE', 'Update Cameras', 'Update cameras', 1),
('CAMERA_DELETE', 'Delete Cameras', 'Deactivate cameras', 1),
('CAMERA_LIVE', 'Live View', 'View live camera streams', 1),
('CAMERA_RECORDING', 'Recording', 'Start and manage recordings', 1),
('CAMERA_PLAYBACK', 'Playback', 'Play recorded video', 1),
('CAMERA_PTZ', 'PTZ', 'Control PTZ cameras', 1),
('LICENSE_VIEW', 'View License', 'View license information', 1),
('LICENSE_ACTIVATE', 'Activate License', 'Activate a license', 1),
('AUDIT_VIEW', 'View Audit Logs', 'View audit logs', 1);

INSERT OR IGNORE INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'SuperAdmin';

INSERT OR IGNORE INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
JOIN Permissions p
WHERE r.Name = 'Administrator'
AND p.Code IN (
    'SYSTEM_VIEW',
    'USER_VIEW', 'USER_CREATE', 'USER_UPDATE', 'USER_DELETE',
    'CAMERA_VIEW', 'CAMERA_CREATE', 'CAMERA_UPDATE', 'CAMERA_DELETE',
    'CAMERA_LIVE', 'CAMERA_RECORDING', 'CAMERA_PLAYBACK', 'CAMERA_PTZ',
    'LICENSE_VIEW', 'LICENSE_ACTIVATE',
    'AUDIT_VIEW'
);

INSERT OR IGNORE INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
JOIN Permissions p
WHERE r.Name = 'Operator'
AND p.Code IN (
    'SYSTEM_VIEW',
    'CAMERA_VIEW',
    'CAMERA_LIVE',
    'CAMERA_RECORDING',
    'CAMERA_PLAYBACK',
    'CAMERA_PTZ'
);

INSERT OR IGNORE INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
JOIN Permissions p
WHERE r.Name = 'Viewer'
AND p.Code IN (
    'SYSTEM_VIEW',
    'CAMERA_VIEW',
    'CAMERA_LIVE',
    'CAMERA_PLAYBACK'
);
