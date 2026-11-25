using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Mews.Eet.Dto
{
    public class Certificate : IDisposable
    {
        public Certificate(string password, byte[] data, bool useMachineKeyStore = false)
        {
            Password = password;
            KeyStorageFlags = GetKeyStorageFlags(useMachineKeyStore);
            Data = data;
            X509Certificate2 = new X509Certificate2(Data, Password, KeyStorageFlags);
            try
            {
                Key = X509Certificate2.GetRSAPrivateKey()
                    ?? throw new ArgumentException("The provided certificate does not have any RSA private keys.");
            }
            catch
            {
                X509Certificate2.Dispose();
                throw;
            }
            UseMachineKeyStore = useMachineKeyStore;
        }

        public string Password { get; }

        public byte[] Data { get; }

        public bool UseMachineKeyStore { get; }

        public RSA Key { get; }

        public X509Certificate2 X509Certificate2 { get; }

        private X509KeyStorageFlags KeyStorageFlags { get; }

        private X509KeyStorageFlags GetKeyStorageFlags(bool useMachineKeyStore)
        {
            if (useMachineKeyStore)
            {
                return X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet;
            }

            return X509KeyStorageFlags.Exportable;
        }

        public void Dispose()
        {
            Key.Dispose();
            X509Certificate2.Dispose();
        }
    }
}
