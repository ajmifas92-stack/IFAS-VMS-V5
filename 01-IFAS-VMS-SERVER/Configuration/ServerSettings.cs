namespace IFAS.Server.Configuration;
public sealed class ServerSettings
{
    public string Name { get; set; } = "IFAS VMS Server";
    public string Host { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 5080;
    public string DatabasePath { get; set; } = "Data/ifas-vms.db";
}
