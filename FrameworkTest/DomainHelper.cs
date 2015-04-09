using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Provides a helper class for appdomains.
/// 
/// Core used from http://devdale.blogspot.com.br/2007/10/getting-list-of-loaded-appdomains.html
/// 
/// </summary>
public static class DomainHelper
{
    /// <summary>
    /// Gets all of the application domains that are 
    /// currently loaded in the application process.
    /// </summary>
    public static AppDomain[] LoadedDomains
    {
        get
        {
            List<AppDomain> loadedDomains = new List<AppDomain>();
            ICorRuntimeHost runtimeHost = (ICorRuntimeHost)(new CorRuntimeHost());

            try
            {
                IntPtr enumeration = IntPtr.Zero;
                runtimeHost.EnumDomains(out enumeration);

                try
                {
                    object nextDomain = null;
                    runtimeHost.NextDomain(enumeration, ref nextDomain);

                    while (nextDomain != null)
                    {
                        loadedDomains.Add((AppDomain)nextDomain);
                        nextDomain = null;
                        runtimeHost.NextDomain(enumeration, ref nextDomain);
                    }
                }
                finally
                {
                    runtimeHost.CloseEnum(enumeration);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(runtimeHost);
            }

            return loadedDomains.ToArray();
        }
    }

    [ComImport]
    [Guid("CB2F6723-AB3A-11d2-9C40-00C04FA30A3E")]
    private class CorRuntimeHost// : ICorRuntimeHost
    {
    }

    [Guid("CB2F6722-AB3A-11D2-9C40-00C04FA30A3E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ICorRuntimeHost
    {
        void CreateLogicalThreadState();
        void DeleteLogicalThreadState();
        void SwitchInLogicalThreadState();
        void SwitchOutLogicalThreadState();
        void LocksHeldByLogicalThread();
        void MapFile();
        void GetConfiguration();
        void Start();
        void Stop();
        void CreateDomain();
        void GetDefaultDomain();
        void EnumDomains(out IntPtr enumHandle);
        void NextDomain(IntPtr enumHandle, [MarshalAs(UnmanagedType.IUnknown)]ref object appDomain);
        void CloseEnum(IntPtr enumHandle);
        void CreateDomainEx();
        void CreateDomainSetup();
        void CreateEvidence();
        void UnloadDomain();
        void CurrentDomain();
    }
}
