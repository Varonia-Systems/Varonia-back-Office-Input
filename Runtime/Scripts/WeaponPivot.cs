using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VaroniaBackOffice
{
    public class WeaponPivot : MonoBehaviour
    {

        public IEnumerator Start()
        {
            yield return new WaitUntil(() => VaroniaInput.Instance != null);
            yield return new WaitUntil(() => VaroniaInput.Instance.Pivot != null);

            transform.localPosition = VaroniaInput.Instance.Pivot.localPosition;
            transform.localRotation = VaroniaInput.Instance.Pivot.localRotation;
        }

    }
}
