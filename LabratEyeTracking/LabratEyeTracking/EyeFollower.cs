using System;
using UnityEngine;

namespace LabratEyeTracking
{
    public class EyeDriver : MonoBehaviour
    {
        private bool set = false;
        private Transform playerHead;

        private void OnEnable()
        {
            if(gameObject.GetComponent<Collider>() != null)
                Destroy(gameObject.GetComponent<Collider>());
        }

        private void FixedUpdate()
        {
            if (playerHead != null)
            {
                Vector3 origin = playerHead.position + UniversalEyeData.CombinedEye.origin;
                Vector3 direction = playerHead.forward + UniversalEyeData.CombinedEye.direction;
                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
                {
                    Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                    transform.position = hit.transform.position;
                }
            }
            else
            {
                playerHead = GameHelper.FindPlayerModel().transform.Find("SteamVRObjects").Find("VRCamera");
                if (!set && playerHead != null)
                {
                    set = true;
                    playerHead.position = Vector3.zero;
                    playerHead.rotation = new Quaternion(0, 0, 0, 0);
                    LogHelper.Debug("Found Player Head!");
                }
            }
        }
    }
}