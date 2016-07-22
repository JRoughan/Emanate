using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace Emanate.Core
{
    public static class SecureStorage
    {
        public static void StoreString(Guid id, string key, string text)
        {
            Log.Information($"=> SecureStorage.StoreString({id}, {key})");
            var isoStore = IsolatedStorageFile.GetMachineStoreForAssembly();
            using (var isoStream = new IsolatedStorageFileStream($"{id}-{key}", FileMode.Create, isoStore))
            {
                using (var writer = new StreamWriter(isoStream))
                {
                    var unencodedBytes = Encoding.UTF8.GetBytes(text);
                    byte[] encodedBytes = ProtectedData.Protect(unencodedBytes, null, DataProtectionScope.LocalMachine);
                    var encodedString = Convert.ToBase64String(encodedBytes);

                    writer.WriteLine(encodedString);
                }
            }
        }

        public static string GetString(Guid id, string key)
        {
            Log.Information($"=> SecureStorage.GetString({id}, {key})");
            var isoStore = IsolatedStorageFile.GetMachineStoreForAssembly();
            using (var isoStream = new IsolatedStorageFileStream($"{id}-{key}", FileMode.OpenOrCreate, isoStore))
            {
                using (var reader = new StreamReader(isoStream))
                {
                    var encodedString = reader.ReadToEnd();

                    var encodedBytes = Convert.FromBase64String(encodedString);
                    byte[] unencodedBytes = ProtectedData.Unprotect(encodedBytes, null, DataProtectionScope.LocalMachine);
                    return Encoding.UTF8.GetString(unencodedBytes);
                }
            }
        }
    }
}
