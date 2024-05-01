using UnityEngine;

public class ScreenPos : MonoBehaviour
{
    void Update()
    {
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log(pos);
    }
}