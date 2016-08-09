using System;

namespace FourSeafile.ViewModel.Mock
{
    public static class MockHelper
    {
        public static void Throw()
        {
            throw new NotImplementedException(Localization.NotInDemo);
        }
    }
}
