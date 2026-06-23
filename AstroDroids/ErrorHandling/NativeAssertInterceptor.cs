using Hexa.NET.ImGui;
using System;
using System.Runtime.InteropServices;

namespace AstroDroids.ErrorHandling
{
    public static class NativeErrorHandler
    {
        [DllImport("ucrtbase.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _set_error_mode(int mode);

        public static void Setup()
        {
            try
            {
                //Tells ucrtbase to create dialog boxes for asserts that ImGui generates, otherwise the errors aren't shown anywhere
                _set_error_mode(2);

                //Tell ImGui that we don't want it to throw asserts for recoverable errors, it will instead show a tooltip in the game itself
                ImGui.GetIO().ConfigErrorRecoveryEnableAssert = false;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
