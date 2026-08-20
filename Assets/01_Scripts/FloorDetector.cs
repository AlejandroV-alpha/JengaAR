using UnityEngine;

public class FloorDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Block>(out Block block))
        {
            JengaManager.Instance.TowerFell();
        }
    }
}
