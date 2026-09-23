using System.Security.Cryptography;

namespace IFAS.VMS.Updater;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("IFAS VMS Updater");
            Console.WriteLine("Usage: IFAS.VMS.Updater.exe verify <file> <sha256>");
            Console.WriteLine("       IFAS.VMS.Updater.exe version");
            return 0;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "version":
                Console.WriteLine("IFAS VMS Updater 1.0.0");
                return 0;

            case "verify":
                if (args.Length < 3) return 2;
                if (!File.Exists(args[1])) return 3;

                var actual = Convert.ToHexString(
                    SHA256.HashData(File.ReadAllBytes(args[1]))).ToLowerInvariant();
                var expected = args[2].Trim().ToLowerInvariant();

                Console.WriteLine($"Expected: {expected}");
                Console.WriteLine($"Actual:   {actual}");

                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(actual),
                    Convert.FromHexString(expected)) ? 0 : 4;

            default:
                return 1;
        }
    }
}
