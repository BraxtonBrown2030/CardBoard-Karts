using UnityEngine;
using UnityEngine.InputSystem;

public class Chat_car_controller : MonoBehaviour
{
    [Header("Suspension Settings")]
    public float springStrength = 8000f;
    public float damping = 1500f;
    public float restLength = 0.5f;
    public float maxSuspensionLength = 1f;

    [Header("Wheel Settings")]
    public Transform[] wheels;
    public LayerMask groundLayer;
    public bool debugRay = true;

    [Header("Steering Settings")]
    public Transform tiretransform;
    public Rigidbody carrigidbody;

    private Rigidbody rb;

    public InputActionReference foward;

    /*
     * 
     * car controller default settings
     *                  inspector / script
     * spring stiffness: 800 / 8000
     * damping: 80 / 1500
     * rest length: 1 / 0.5
     * max suspension length: 1.5 / 1
     *
     * ground layer: drivable / drivable
     *
     * tire transform: car_main transform
     * car rigid body: car_main rigid body
     *
     * forward input action: player/move
     * 
     */
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (var wheel in wheels)
        {
            HandleSuspension(wheel);
        }

        steering();
    }

    void HandleSuspension(Transform wheel)
    {
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

                // Debug.Log($"Wheel: {wheel.name}, Compression: {compression}, Total Force: {totalForce}");
            }
        }
        else
        {
            if(debugRay)
            {
                Debug.DrawRay(wheel.position, -transform.up * maxSuspensionLength, Color.red);
                rb.mass = 40;
                return;
            }
        }
    }

    public void steering()
    {

        Vector2 moveInput = foward.action.ReadValue<Vector2>();
        
        if (moveInput.y > 0.1f)
        {
            carrigidbody.AddForceAtPosition(tiretransform.forward * 1000, tiretransform.position);
        }
        else if (moveInput.y < -0.1f)
        {
            carrigidbody.AddForceAtPosition(-tiretransform.forward * 1000, tiretransform.position);
        }
        
        if (moveInput.x > 0.1f)
        {
            carrigidbody.AddTorque(Vector3.up * 50);
        }
        else if (moveInput.x < -0.1f)
        {
            
            carrigidbody.AddTorque(Vector3.up * -50);
        }
        else
        {
            carrigidbody.angularVelocity = Vector3.zero;
        }
    }
}