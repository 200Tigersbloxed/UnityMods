using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections;
using MelonLoader;
using UnityEngine;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
    class TextFileManager : HRManager
    {
        bool shouldUpdate = false;
        string pubFe = String.Empty;
        int HR = 0;

        private Thread _thread = null;

        bool HRManager.Init(string fileLocation)
        {
            bool fe = File.Exists(fileLocation);
            if (fe)
            {
                LogHelper.Log("TextFileManager", "Found text file!");
                pubFe = fileLocation;
                shouldUpdate = true;
                StartThread();
            }
            else
                LogHelper.Error("TextFileManager", "Failed to find text file!");
            return fe;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                while (shouldUpdate)
                {
                    bool failed = false;
                    int tempHR = 0;
                    // get text
                    string text = String.Empty;
                    try { text = File.ReadAllText(pubFe); } catch (Exception e) { LogHelper.Error("TextFile", "Failed to find Text File! Exception: " + e); failed = true; }
                    // cast to int
                    if (!failed)
                        try { tempHR = Convert.ToInt32(text); } catch (Exception e) { LogHelper.Error("TextFile", "Failed to parse to int! Exception: " + e); }
                    HR = tempHR;
                    Thread.Sleep(500);
                }
            });
            _thread.Start();
        }

        void HRManager.Stop()
        {
            shouldUpdate = false;
            VerifyClosedThread();
        }

        int HRManager.GetHR() => HR;
        public bool IsOpen() => shouldUpdate;
        public bool IsActive() => IsOpen();
    }
}
