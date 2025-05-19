using System.Collections;
using UnityEngine;

public class FollowGun : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitUntil(() => VaroniaBackOffice.VaroniaInput.Instance != null);
        yield return new WaitUntil(() => VaroniaBackOffice.VaroniaInput.Instance.Pivot != null);

        while (true)
        {
            transform.position = VaroniaBackOffice.VaroniaInput.Instance.Pivot.position;
            transform.rotation = VaroniaBackOffice.VaroniaInput.Instance.Pivot.rotation;

            yield return null;

        }

    }


}
