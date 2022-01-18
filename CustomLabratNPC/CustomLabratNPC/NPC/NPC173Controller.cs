using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace CustomLabratNPC.NPC
{
    /// <summary>
    /// NPCController that handles NPC creation of SCP-173
    /// 
    /// The SCP-173 model consists of the following hierarchy:
    /// SCP173Final (Model Container)
    /// ├─ 173Rebar (???)
    /// ├─ Scene_Root (The Mesh)
    ///     ├─ MeshFilter
    ///     ├─ MeshRenderer
    ///     ├─ MeshCollider
    ///     ├─ CapsuleCollider
    /// ├─ 173Sound (Audio)
    /// ├─ Peanut Teleport Random Intervals (???)
    /// ├─ Raycasts (Movement and Player Detection)
    /// ├─ Doom (Player Killing)
    /// </summary>
    public class NPC173Controller : MonoBehaviour, INPCController
    {
        // Info and Cache
        public GameObject HookedNPC { get; set; }
        public NPCEnum NPCType => NPCEnum.SCP173;
        public CustomNPCDescriptor Descriptor { get; set; }
        private NPC173Follower _follower;
        public GameObject TargetSCP { get; set; }
        public GameObject Scene_Root => TargetSCP.transform.Find("Scene_Root").gameObject;
        private Renderer objectRenderer;
        private Animator _animator;

        // Tools
        public bool IsTargetSCP(GameObject scp) => scp.CompareTag("173");
        public bool IsRegistered { get; set; }

        // Events
        public event Action<NPCEvents, INPCController> OnSCP = (events, controller) => { };

        private Vector3 _recentPosition = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Vector3 SCPMoveDistance
        {
            get
            {
                return _recentPosition - _lastPosition;
            }
            set
            {
                _lastPosition = _recentPosition;
                _recentPosition = value;
            }
        }

        public void ProxyEventInvoke(NPCEvents npce)
        {
            OnSCP.Invoke(npce, this);
            if (_animator != null)
            {
                switch (npce)
                {
                    case NPCEvents.Instantiated:
                        Descriptor.FireTrigger("Created");
                        break;
                    case NPCEvents.Destroyed:
                        Descriptor.FireTrigger("Destroyed");
                        break;
                }
            }
        }

        private void HandleNewDescriptor(CustomNPCDescriptor newDescriptor)
        {
            newDescriptor.isInstanced = true;
            // Handle Events
            OnSCP += (events, controller) =>
            {
                newDescriptor.OnSCPEvent.Invoke(events);
            };
            // Find the Animator
            try
            {
                _animator = newDescriptor.gameObject.GetComponent<Animator>();
            }
            catch (Exception)
            {
                LogHelper.Warn($"{newDescriptor.gameObject.name} does not have an animator!");
            }
            // Handle NPC Creation
            // Hide the Scene_Root
            if (Scene_Root != null)
            {
                (objectRenderer = Scene_Root.GetComponent<Renderer>()).forceRenderingOff = true;
            }
            // Attach the special follower component
            _follower = gameObject.AddComponent<NPC173Follower>();
            _follower.Controller = this;
            _follower.SceneRoot = Scene_Root.transform;
            _follower.npcd = newDescriptor;
            _follower.didInitValues = true;
        }

        IEnumerator EnableWait()
        {
            LogHelper.Debug("Waiting for values...");
            while (TargetSCP == null || Descriptor == null || HookedNPC == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
            // HookedNPC will be set before enabled
            // No Register is required
            if (IsTargetSCP(TargetSCP))
            {
                OnSCP.Invoke(NPCEvents.Instantiated, this);
                HookedNPC.transform.parent = TargetSCP.transform;
                HandleNewDescriptor(Descriptor);
                IsRegistered = true;
            }
            else
            {
                LogHelper.Warn($"Target SCP {NPCType.ToString()} is not valid for this class!");
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            MelonCoroutines.Start(EnableWait());
        }

        private void OnDisable()
        {
            if (IsTargetSCP(TargetSCP))
            {
                OnSCP.Invoke(NPCEvents.Destroyed, this);
                NPCControllerCache.Deregister(this);
                objectRenderer.forceRenderingOff = false;
                IsRegistered = false;
            }
        }

        private void FixedUpdate()
        {
            SCPMoveDistance = transform.position;
        }

        private void Update()
        {
            if (IsRegistered)
            {
                if(objectRenderer != null)
                    objectRenderer.forceRenderingOff = true;
                Descriptor.SetParameter("IsMoving", SCPMoveDistance != Vector3.zero && !LabratTools.isLocalPlayerDead);
                Descriptor.SetParameter("LocalPlayerDead", LabratTools.isLocalPlayerDead);
            }
        }

        private void OnDestroy()
        {
            NPCControllerCache.Deregister(this);
            Destroy(HookedNPC);
        }
    }

    /// <summary>
    /// Special class crafted to accompany an NPC targeted towards SCP-173
    /// </summary>
    public class NPC173Follower : MonoBehaviour
    {
        public NPC173Controller Controller;
        public Transform SceneRoot;
        public CustomNPCDescriptor npcd;
        public bool didInitValues;
        
        private void Update()
        {
            if (didInitValues)
            {
                if (npcd != null && Controller != null && SceneRoot != null)
                {
                    var transform1 = npcd.transform;
                    transform1.position = SceneRoot.position;
                    transform1.rotation = SceneRoot.parent.rotation;
                }
                else
                {
                    LogHelper.Warn($"npcd ({npcd}) or cont ({Controller}) or SceneRoot ({SceneRoot}) is null!");
                }
            }
        }
    }

    public class NPC173Protector : MonoBehaviour
    {
        private NPCEnum NPCType;
        private string NPCName;
        private bool didInit;
        
        public void Init(NPCEnum type, string npcname)
        {
            NPCType = type;
            NPCName = npcname;
            didInit = true;
        }
        
        private void Update()
        {
            List<GameObject> duplicates = new List<GameObject>();
            if(didInit)
                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject obj = transform.GetChild(i).gameObject;
                    if (LabratTools.HasComponent<NPC173Controller>(obj) ||
                        LabratTools.HasComponent<NPC173Follower>(obj) ||
                        LabratTools.HasComponent<CustomNPCDescriptor>(obj) || obj.name.Contains(NPCName))
                    {
                        // Found Duplicate
                        duplicates.Add(obj);
                    }
                }

            if (duplicates.Count > 1)
            {
                bool skipdupe = false;
                foreach (GameObject duplicate in duplicates)
                {
                    if (!skipdupe)
                        skipdupe = true;
                    else
                    {
                        Destroy(duplicate);
                        LogHelper.Debug("Removed Duplicate NPC");
                    }
                }
            }
        }
    }
}