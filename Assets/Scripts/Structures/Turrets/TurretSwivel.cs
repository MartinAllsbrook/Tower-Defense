using UnityEngine;

public class TurretSwivel : MonoBehaviour
{
    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}