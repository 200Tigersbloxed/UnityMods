//using GenericHRLib; Replaced with Reflection
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
    public class WinBLEGATTManager : HRManager
    {
        private Assembly generichr_assembly = null;
        private Type generichr_type = null;
        private static object global_generichr_instance = null;

        private Thread _thread = null;

        public static int HR { get; private set; } = 0;
        public static bool isConnected { get; private set; } = false;

        public static void HandleHRUpdated(object structInstance)
        {
            int eventHR = (int)structInstance.GetType().GetProperty("BeatsPerMinute").GetValue(global_generichr_instance);
            HR = eventHR;
        }

        public static void HandleHRDisconnected() => isConnected = false;

        private void load_generichr_resource()
        {
            byte[] assemblyBytes = null;
            byte[] assemblySymbols = null;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("HRtoVRChat.Libs.GenericHRLib.dll"))
            {
                if (stream != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        assemblyBytes = ms.ToArray();
                        LogHelper.Debug("WinBLEGATTManager", "Loaded GenericHRLib Assembly!");
                    }
                }
                else
                    LogHelper.Error("WinBLEGATTManager", "stream is null! Does the ManifestResource exist?");
            }
            using (var stream = assembly.GetManifestResourceStream("HRtoVRChat.Libs.GenericHRLib.pdb"))
            {
                if (stream != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        assemblySymbols = ms.ToArray();
                        LogHelper.Debug("WinBLEGATTManager", "Loaded GenericHRLib Assembly Symbols!");
                    }
                }
                else
                    LogHelper.Error("WinBLEGATTManager", "stream is null! Does the ManifestResource exist?");
            }
            if (assemblyBytes != null && assemblySymbols != null)
                Assembly.Load(assemblyBytes, assemblySymbols);
        }

        private object create_generichr_instance()
        {
            if(generichr_assembly != null)
            {
                generichr_type = generichr_assembly.GetType("GenericHRLib.GenericHRDevice");
                object newInstance = Activator.CreateInstance(generichr_type);
                global_generichr_instance = newInstance;
                LogHelper.Debug("WinBLEGATTManager", "GenericHRDevice created!");
                return newInstance;
            }
            else
            {
                LogHelper.Error("WinBLEGATTManager", "generichr_assembly is null!");
                global_generichr_instance = null;
                return null;
            }
        }

        private void generichrdevice_register_events(object reference)
        {
            // HeartRateUpdated
            EventInfo hrupdated = generichr_type.GetEvent("HeartRateUpdated");
            Delegate hruHandler = Delegate.CreateDelegate(hrupdated.EventHandlerType, null, this.GetType().GetMethod("HandleHRUpdated"));
            hrupdated.GetAddMethod().Invoke(reference, new[] { hruHandler });
            // HeartRateDisconnected
            EventInfo hrdisconnected = generichr_type.GetEvent("HeartRateDisconnected");
            Delegate hrdHandler = Delegate.CreateDelegate(hrdisconnected.EventHandlerType, null, this.GetType().GetMethod("HandleHRDisconnected"));
            hrdisconnected.GetAddMethod().Invoke(reference, new[] { hrdHandler });
            LogHelper.Debug("WinBLEGATTManager", "Registered GenericHRDevice Events!");
        }

        private bool generichrdevice_findandconnect(object reference)
        {
            bool didConnect = false;
            MethodInfo method = generichr_type.GetMethod("FindAndConnect");
            object invokedMethod = null;
            try { invokedMethod = method.Invoke(reference, null); didConnect = true; }catch(Exception e) 
            {
                LogHelper.Error("WinBLEGATTManager", "Failed to FindAndConnect() devices! Exception: " + e);
                didConnect = false;
            }
            return didConnect;
        }

        private void generichrdevice_dispose(object reference)
        {
            MethodInfo disposeMethod = generichr_type.GetMethod("Dispose");
            object invokedMethod = disposeMethod.Invoke(reference, null);
        }

        private void VerifyClosedDevice()
        {
            if (global_generichr_instance != null)
                generichrdevice_dispose(global_generichr_instance);
            global_generichr_instance = null;
        }

        private void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
                _thread = null;
            }
        }

        public bool Init(string empty)
        {
            object device = null;
            bool didConnect = false;
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (generichr_assembly == null)
                    load_generichr_resource();
                if (isConnected)
                    VerifyClosedDevice();
                if (generichr_assembly != null)
                {
                    device = create_generichr_instance();
                    generichrdevice_register_events(global_generichr_instance);
                    didConnect = generichrdevice_findandconnect(device);
                }
                else
                    LogHelper.Error("WinBLEGATTManager", "GenericHR Assembly is null!");
                while (isConnected)
                {
                    // Events handle everything, we just need to keep the thread alive to avoid GC errors
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
            return didConnect;
        }

        public bool IsOpen() => isConnected;
        public bool IsActive() => isConnected;
        public int GetHR() => HR;

        public void Stop()
        {
            VerifyClosedDevice();
            VerifyClosedThread();
        }
    }

    /*
    public class WinBLEGATTManager : HRManager
    {
        private Thread _thread = null;
        private GenericHRDevice ghrd = null;

        public int HR { get; private set; } = 0;
        public bool isDeviceConnected { get; private set; } = false;

        public bool Init(string bruh)
        {
            StartThread();
            return isDeviceConnected;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void VerifyClosedDevice()
        {
            if (ghrd != null)
                ghrd.Dispose();
            ghrd = null;
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                VerifyClosedDevice();
                ghrd = new GenericHRDevice();
                try { ghrd.FindAndConnect(); isDeviceConnected = true; } catch (HRDeviceException e) { LogHelper.Error("HRManagers/WinBLEGATTManager.cs", 
                    "Failed to FindAndConnect() to HR Device! Exception: " + e); isDeviceConnected = false; }
                if (isDeviceConnected)
                {
                    ghrd.HeartRateUpdated += ghrd_HeartRateUpdated;
                    ghrd.HeartRateDisconnected += ghrd_HeartRateDisconnected;
                }
                while (isDeviceConnected)
                {
                    // Events handle everything, just need to keep this thread alive to avoid GC errors
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void ghrd_HeartRateDisconnected() => isDeviceConnected = false;
        private void ghrd_HeartRateUpdated(HeartRateReading reading) => HR = reading.BeatsPerMinute;

        public bool IsOpen() => isDeviceConnected;
        public bool IsActive() => isDeviceConnected;
        public int GetHR() => HR;

        public void Stop()
        {
            VerifyClosedDevice();
            VerifyClosedThread();
            HR = 0;
            isDeviceConnected = false;
        }
    }
    */
}
