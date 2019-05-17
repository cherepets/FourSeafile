using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.UI.Popups;

namespace FourSeafile
{
    public static class WindowsHello
    {
        private const string SaltName = "Salt";
        private const string Challenge = "{52555D5F-9E09-4265-81E5-8867A41035F9}";

        public static string Sign { get; private set; }

        public static bool IsVerified => Settings.Local.UseWindowsHello && Sign != null;

        public static async Task<bool> VerifyAsync()
        {
            var supported = await KeyCredentialManager.IsSupportedAsync();
            if (!supported)
                await new MessageDialog(Localization.HelloUnavialable, Localization.Warning).ShowAsync();
            else
            {
                while (true)
                {
                    var result = await KeyCredentialManager.OpenAsync(SaltName);
                    switch (result.Status)
                    {
                        case KeyCredentialStatus.NotFound:
                            return await GenerateSaltAsync();
                        case KeyCredentialStatus.Success:
                            Sign = await SignAsync(result.Credential);
                            return Sign != null;
                        case KeyCredentialStatus.UserCanceled:
                            return false;
                        case KeyCredentialStatus.CredentialAlreadyExists:
                        case KeyCredentialStatus.SecurityDeviceLocked:
                        case KeyCredentialStatus.UserPrefersPassword:
                            await new MessageDialog(Localization.HelloError, Localization.Warning).ShowAsync();
                            return false;
                        default:
                            await new MessageDialog(Localization.HelloRetry, Localization.Warning).ShowAsync();
                            break;
                    }
                }
            }
            return false;
        }

        public static async Task<bool> GenerateSaltAsync()
        {
            var supported = await KeyCredentialManager.IsSupportedAsync();
            if (!supported)
                await new MessageDialog(Localization.HelloUnavialable, Localization.Warning).ShowAsync();
            else
            {
                try
                {
                    var result = await KeyCredentialManager.RequestCreateAsync(SaltName, KeyCredentialCreationOption.ReplaceExisting);
                    if (result.Status == KeyCredentialStatus.Success)
                    {
                        Sign = await SignAsync(result.Credential);
                        return Sign != null;
                    }
                    await new MessageDialog(Localization.HelloError, Localization.Warning).ShowAsync();
                    return false;
                }
                catch
                {
                    await new MessageDialog(Localization.HelloError, Localization.Warning).ShowAsync();
                    return false;
                }
            }
            return false;
        }

        private static async Task<string> SignAsync(KeyCredential credential)
        {
            try
            {
                var buffer = CryptographicBuffer.ConvertStringToBinary(Challenge, BinaryStringEncoding.Utf8);
                var result = await credential.RequestSignAsync(buffer);
                if (result.Status == KeyCredentialStatus.Success)
                {
                    var signatureBytes = result.Result.ToArray();
                    return Convert.ToBase64String(signatureBytes);
                }
                await new MessageDialog(Localization.HelloError, Localization.Warning).ShowAsync();
                return null;
            }
            catch
            {
                await new MessageDialog(Localization.HelloError, Localization.Warning).ShowAsync();
                return null;
            }
        }
    }
}
