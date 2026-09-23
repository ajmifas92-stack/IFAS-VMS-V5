using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace IFAS.VMS.Client.Services;

public sealed record OnvifDevice(string Address, string Types, string Scopes);

public sealed class OnvifDiscoveryService
{
    public async Task<IReadOnlyList<OnvifDevice>> DiscoverAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        const string probe = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<e:Envelope xmlns:e=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:w=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\" xmlns:d=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\">" +
            "<e:Header><w:MessageID>uuid:ifas-discovery</w:MessageID><w:To>urn:schemas-xmlsoap-org:ws:2005:04:discovery</w:To><w:Action>http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</w:Action></e:Header>" +
            "<e:Body><d:Probe><d:Types>dn:NetworkVideoTransmitter</d:Types></d:Probe></e:Body></e:Envelope>";

        using var udp = new UdpClient(AddressFamily.InterNetwork);
        udp.Client.ReceiveTimeout = (int)timeout.TotalMilliseconds;
        udp.EnableBroadcast = true;
        var target = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 3702);
        var data = Encoding.UTF8.GetBytes(probe.Replace("dn:", "dn:http://www.onvif.org/ver10/network/wsdl/"));
        await udp.SendAsync(data, data.Length, target);

        var end = DateTime.UtcNow + timeout;
        var results = new Dictionary<string, OnvifDevice>(StringComparer.OrdinalIgnoreCase);
        while (DateTime.UtcNow < end && !ct.IsCancellationRequested)
        {
            try
            {
                var remaining = end - DateTime.UtcNow;
                udp.Client.ReceiveTimeout = Math.Max(100, (int)Math.Min(remaining.TotalMilliseconds, 500));
                var r = await udp.ReceiveAsync(ct);
                var xml = Encoding.UTF8.GetString(r.Buffer);
                var xaddrs = Regex.Matches(xml, @"<[^>]*XAddrs[^>]*>(.*?)</[^>]*XAddrs>", RegexOptions.Singleline | RegexOptions.IgnoreCase)
                    .Cast<Match>().SelectMany(m => m.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
                var types = Regex.Match(xml, @"<[^>]*Types[^>]*>(.*?)</[^>]*Types>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
                var scopes = Regex.Match(xml, @"<[^>]*Scopes[^>]*>(.*?)</[^>]*Scopes>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
                foreach (var address in xaddrs.Distinct()) results[address] = new OnvifDevice(address, types, scopes);
            }
            catch (OperationCanceledException) { break; }
            catch (SocketException) { }
        }
        return results.Values.ToList();
    }
}
