-- IFAS VMS initial logical schema
-- SQLite-compatible reference schema.
-- The running Server and License Manager currently manage their own databases.

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Roles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT NOT NULL DEFAULT '',
    IsActive INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Permissions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Description TEXT NOT NULL DEFAULT '',
    IsActive INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL UNIQUE,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    DisplayName TEXT NOT NULL DEFAULT '',
    Email TEXT NOT NULL DEFAULT '',
    RoleId INTEGER NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAtUtc TEXT NOT NULL,
    UpdatedAtUtc TEXT,
    LastLoginAtUtc TEXT,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE IF NOT EXISTS RolePermissions (
    RoleId INTEGER NOT NULL,
    PermissionId INTEGER NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

CREATE TABLE IF NOT EXISTS Cameras (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CameraId TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Host TEXT NOT NULL,
    Port INTEGER NOT NULL DEFAULT 554,
    Protocol TEXT NOT NULL DEFAULT 'RTSP',
    Username TEXT NOT NULL DEFAULT '',
    Password TEXT NOT NULL DEFAULT '',
    StreamUrl TEXT NOT NULL DEFAULT '',
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    CreatedAtUtc TEXT NOT NULL,
    UpdatedAtUtc TEXT
);

CREATE TABLE IF NOT EXISTS Licenses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    LicenseId TEXT NOT NULL UNIQUE,
    LicenseKey TEXT NOT NULL UNIQUE,
    MaxUsers INTEGER NOT NULL,
    MaxCameras INTEGER NOT NULL,
    IssuedAtUtc TEXT NOT NULL,
    ExpiresAtUtc TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    IsRevoked INTEGER NOT NULL DEFAULT 0,
    ServerBinding TEXT NOT NULL DEFAULT '',
    FeaturesJson TEXT NOT NULL DEFAULT '[]',
    LicensePayload TEXT NOT NULL DEFAULT '',
    Signature TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS AuditLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EventType TEXT NOT NULL,
    Username TEXT NOT NULL DEFAULT '',
    Description TEXT NOT NULL DEFAULT '',
    IpAddress TEXT NOT NULL DEFAULT '',
    CreatedAtUtc TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_Users_RoleId
ON Users(RoleId);

CREATE INDEX IF NOT EXISTS IX_Cameras_Name
ON Cameras(Name);

CREATE INDEX IF NOT EXISTS IX_AuditLogs_CreatedAtUtc
ON AuditLogs(CreatedAtUtc);
