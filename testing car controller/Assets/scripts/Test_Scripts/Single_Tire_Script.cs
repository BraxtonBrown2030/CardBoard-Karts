using UnityEngine;
using UnityEngine.InputSystem;

public class Single_Tire_Script : MonoBehaviour
{
   
    [Header("Suspension Settings")]
    public float springStrength = 8000f;
    public float damping = 1500f;
    public float restLength = 0.5f;
    public float maxSuspensionLength = 1f;

    [Header("Wheel Settings")]
    public Transform wheel;
    public LayerMask groundLayer;
    public bool debugRay = true;

    [Header("Steering Settings")]
    public Rigidbody rb;

    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (wheel != null)
        {
            HandleSuspension(wheel);
        }
        else
        {
            Debug.LogError("Wheel transform is not assigned.");
        }
    }

    void HandleSuspension(Transform wheel)
    {
        if (wheel == null)
        {
            Debug.LogError("Wheel transform is null in HandleSuspension.");
            return;
        }

        Ray ray = new Ray(wheel.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, maxSuspensionLength, groundLayer))
        {
            float distance = hit.distance;
            float compression = restLength - distance;

            if (compression > 0)
            {
                float springForce = springStrength * compression;
                float velocity = Vector3.Dot(rb.GetPointVelocity(wheel.position), -transform.up);
                float dampingForce = damping * velocity;

                Vector3 totalForce = (springForce + dampingForce) * transform.up;
                rb.AddForceAtPosition(totalForce, wheel.position);

                if (debugRay)
                    Debug.DrawRay(wheel.position, -transform.up * hit.distance, Color.green);

                Debug.Log($"Wheel: {wheel.name}, Compression: {compression}, Total Force: {totalForce}");
            }
        }
        else
        {
            if (debugRay)
            {
                Debug.DrawRay(wheel.position, -transform.up * maxSuspensionLength, Color.red);
            }
        }
    }
}