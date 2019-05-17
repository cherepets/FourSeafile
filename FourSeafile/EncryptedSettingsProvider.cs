using System.Collections.Generic;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;

namespace FourSeafile
{
    public class EncryptedSettingsProvider : SettingsProvider
    {
        private readonly string _deviceId = GetDeviceId();

        public EncryptedSettingsProvider()
        {
            Container = ApplicationData.Current.LocalSettings;
        }

        public override IDictionary<string, object> GetSettings()
        {
            var dict = new Dictionary<string, object>();
            var values = Container.Values;
            foreach (var value in values)
                if (value.Value is byte[])
                    dict.Add(value.Key, Crypt.Decrypt(value.Value as byte[], _deviceId));
            return dict;
        }

        public override void SetSetting(string key, object value)
            => Container.Values[key] = value != null 
            ? Crypt.Encrypt(value.ToString(), _deviceId)
            : null;

        public override bool Readable => true;

        public override bool Writeable => true;

        private static string GetDeviceId()
            => new EasClientDeviceInformation()
            .Id.ToString();

        private static class Crypt
        {
            public static string Salt
                => WindowsHello.IsVerified
                    ? WindowsHello.Sign
                    : "salt";

            public static byte[] Encrypt(string plainText, string key)
            {
                if (Salt == null) return null;

                try
                {
                    var pwBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
                    var saltBuffer = CryptographicBuffer.ConvertStringToBinary(Salt, BinaryStringEncoding.Utf16LE);
                    var plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf16LE);
                    var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
                    var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);
                    var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
                    var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
                    var derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);
                    var saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);
                    var keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
                    var saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);
                    var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
                    var symmKey = symProvider.CreateSymmetricKey(keyMaterial);
                    var resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, saltMaterial);
                    CryptographicBuffer.CopyToByteArray(resultBuffer, out byte[] result);
                    return result;
                }
                catch
                {
                    return null;
                }
            }

            public static string Decrypt(byte[] encryptedData, string key)
            {
                if (Salt == null) return null;

                try
                {
                    var pwBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
                    var saltBuffer = CryptographicBuffer.ConvertStringToBinary(Salt, BinaryStringEncoding.Utf16LE);
                    var cipherBuffer = CryptographicBuffer.CreateFromByteArray(encryptedData);
                    var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
                    var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);
                    var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
                    var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
                    var derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);
                    var saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);
                    var keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
                    var saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);
                    var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
                    var symmKey = symProvider.CreateSymmetricKey(keyMaterial);
                    var resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, saltMaterial);
                    var result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16LE, resultBuffer);
                    return result;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
