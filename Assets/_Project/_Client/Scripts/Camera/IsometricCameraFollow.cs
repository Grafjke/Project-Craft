using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{

    [Header("Target to Follow")]
    [SerializeField] private Transform target; 

    [Header("Camera Offset")]
    [SerializeField] private Vector3 offset = new Vector3(-10f, 10f, -10f);

    [Header("Smoothness")]
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 _currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
       
    }
}




