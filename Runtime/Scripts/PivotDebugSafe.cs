using UnityEngine;


[ExecuteAlways]
public class PivotDebugSafe : MonoBehaviour
{
    void Update()
    {
        if (transform.parent != null)
        {
            Vector3 parentScale = transform.parent.lossyScale;
            // Pour éviter les divisions par zéro :
            parentScale.x = parentScale.x == 0 ? 0.0001f : parentScale.x;
            parentScale.y = parentScale.y == 0 ? 0.0001f : parentScale.y;
            parentScale.z = parentScale.z == 0 ? 0.0001f : parentScale.z;

            transform.localScale = new Vector3(
                1f / parentScale.x,
                1f / parentScale.y,
                1f / parentScale.z
            );
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
