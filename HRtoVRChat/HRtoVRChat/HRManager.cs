using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRtoVRChat
{
    public interface HRManager
    {
        bool Init(string d1);
        int GetHR();
        void Stop();
    }
}
