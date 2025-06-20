using System.Collections;
using UnityEngine;

public class FollowGun : MonoBehaviour
{
    void LateUpdate()
        {
            
             if(VaroniaBackOffice.VaroniaInput.Instance == null) return;
             if(VaroniaBackOffice.VaroniaInput.Instance.Pivot == null) return;
            
            transform.position = VaroniaBackOffice.VaroniaInput.Instance.Pivot.position;
            transform.rotation = VaroniaBackOffice.VaroniaInput.Instance.Pivot.rotation;
            
        }
}
