using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using IFAS.LicenseManager.Models;

namespace IFAS.LicenseCreator.Desktop;

public partial class MainWindow : Window
{
    private readonly string _keysDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "IFAS-VMS", "SupplierKeys");

    public MainWindow() => InitializeComponent();

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(CustomerBox.Text)) throw new InvalidOperationException("Customer name is required.");
            if (!int.TryParse(UsersBox.Text, out var users) || users < 1) throw new InvalidOperationException("Maximum users must be a positive number.");
            if (!int.TryParse(CamerasBox.Text, out var cameras) || cameras < 1) throw new InvalidOperationException("Maximum cameras must be a positive number.");
            if (!int.TryParse(DaysBox.Text, out var days) || days < 1) throw new InvalidOperationException("Validity must be a positive number of days.");

            var folder = new OpenFolderDialog { Title = "Select license output folder" };
            if (folder.ShowDialog() != true) return;

            Directory.CreateDirectory(_keysDirectory);
            var privatePath = Path.Combine(_keysDirectory, "ifas-license-private.pem");
            var publicPath = Path.Combine(_keysDirectory, "ifas-license-public.pem");
            using var rsa = RSA.Create(3072);
            if (File.Exists(privatePath)) rsa.ImportFromPem(File.ReadAllText(privatePath));
            else
            {
                File.WriteAllText(privatePath, rsa.ExportPkcs8PrivateKeyPem());
                File.WriteAllText(publicPath, rsa.ExportSubjectPublicKeyInfoPem());
            }

            var issued = DateTime.UtcNow;
            var licenseId = $"IFAS-LIC-{Guid.NewGuid():N}".ToUpperInvariant();
            var licenseKey = $"IFAS-{Convert.ToHexString(RandomNumberGenerator.GetBytes(24))}";
            var customerId = $"IFAS-CUS-{Guid.NewGuid():N}".ToUpperInvariant();
            var features = FeaturesBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            var payloadObject = new { LicenseId = licenseId, LicenseKey = licenseKey, CustomerId = customerId, CustomerName = CustomerBox.Text.Trim(), ContactPerson = ContactBox.Text.Trim(), MaxUsers = users, MaxCameras = cameras, IssuedAtUtc = issued, ExpiresAtUtc = issued.AddDays(days), ServerBinding = BindingBox.Text.Trim(), Features = features };
            var payload = JsonSerializer.Serialize(payloadObject);
            var signature = Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(payload), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            var package = new { payload = payloadObject, signature, algorithm = "RSA-SHA256", formatVersion = 1 };
            var json = JsonSerializer.Serialize(package, new JsonSerializerOptions { WriteIndented = true });
            var safeName = string.Concat(CustomerBox.Text.Trim().Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            var path = Path.Combine(folder.FolderName, $"{safeName}_{licenseId}.ifaslic");
            File.WriteAllText(path, json);
            StatusText.Text = $"LICENSE CREATED\n{path}\nLicense ID: {licenseId}\nPrivate key remains at: {privatePath}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "License creation error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
