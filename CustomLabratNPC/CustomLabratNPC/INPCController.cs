using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomLabratNPC
{
    public interface INPCController
    {
        GameObject HookedNPC { get; set; }
        NPCEnum NPCType { get; }
        CustomNPCDescriptor Descriptor { get; set; }
        GameObject TargetSCP { get; set; }

        bool IsTargetSCP(GameObject scp);
        bool IsRegistered { get; set; }

        event Action<NPCEvents, INPCController> OnSCP;
        void ProxyEventInvoke(NPCEvents npce);
    }

    public static class NPCControllerCache
    {
        public readonly static List<INPCController> _npcControllers = new List<INPCController>();

        public static bool Register(INPCController npcc)
        {
            for (int i = 0; i < npcc.TargetSCP.transform.childCount; i++)
            {
                if (LabratTools.HasComponent<CustomNPCDescriptor>(npcc.TargetSCP.transform.GetChild(i).gameObject))
                    return false;
            }
            return true;
        }
        public static void Deregister(INPCController npcc) => _npcControllers.Remove(npcc);

        [CanBeNull]
        public static INPCController FindAssociatedNPC(INPCController _componentIdentifier) =>
            _npcControllers.FirstOrDefault(x => x == _componentIdentifier);

        [CanBeNull]
        public static INPCController FindAssociatedNPC(CustomNPCDescriptor descriptor) =>
            _npcControllers.FirstOrDefault(x => x.Descriptor == descriptor);
    }
}