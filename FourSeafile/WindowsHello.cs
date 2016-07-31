using System;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;

namespace FourSeafile
{
    public static class WindowsHello
    {
        public static async Task<bool> VerifyAsync()
        {
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();
            if (availability != UserConsentVerifierAvailability.Available)
                await new MessageDialog(Localization.HelloUnavialable, Localization.Warning).ShowAsync();
            else
            {
                while (true)
                {
                    var result = await UserConsentVerifier.RequestVerificationAsync(Localization.HelloAuth);
                    switch (result)
                    {
                        case UserConsentVerificationResult.Verified:
                            return true;
                        case UserConsentVerificationResult.Canceled:
                            return false;
                        case UserConsentVerificationResult.RetriesExhausted:
                        case UserConsentVerificationResult.DeviceNotPresent:
                        case UserConsentVerificationResult.DisabledByPolicy:
                        case UserConsentVerificationResult.NotConfiguredForUser:
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
    }
}
