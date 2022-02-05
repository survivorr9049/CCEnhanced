using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 wishDir;
    Vector3 playerInput;
    CCEnhanced player;
    CameraController fppCamera;
    public enum States{walking, air, crouching, sliding, trimping, wallrunning};
    States state;
    [Header("Keybinds")]
    [SerializeField]KeyCode jump;
    [SerializeField]KeyCode crouch;
    
    [Header("Parameters")]
    [SerializeField]float groundAcceleration;
    [SerializeField]float airAcceleration;
    [SerializeField]float groundSpeed;
    [SerializeField]float airSpeed;
    [SerializeField]float drag;
    [SerializeField]float jumpHeight;   
    [SerializeField]float gravity;
    [SerializeField]float adhesion;
    [SerializeField]float groundDetectionDelay;
    Vector3 velocity;
    Vector3 normal;
    float groundTimer;
    private Vector3 plane = new Vector3(1, 0, 1);
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CCEnhanced>();
        fppCamera = GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        wishDir = GetWishDir();
        player.Move(velocity * Time.deltaTime);
        if(Input.GetKey(KeyCode.Space) && player.isGrounded){
            velocity.y = 0;
            velocity.y += jumpHeight;
        }
        if(player.isGrounded){
            groundTimer += Time.deltaTime;
        }else{
            groundTimer = 0;
        }
        DetermineState();
    }
    void FixedUpdate(){
        if(!player.isGrounded)velocity = ApplyGravity(velocity, gravity * Time.fixedDeltaTime);
        else velocity.y = Mathf.Clamp(velocity.y, -gravity, Mathf.Infinity);
        switch (state){
            case States.walking:
                velocity = GroundMove(wishDir, velocity, groundAcceleration, groundSpeed);
                break;
            case States.air:
                velocity = AirMove(wishDir, velocity, airAcceleration, airSpeed);
                break;
        }
        Debug.Log(velocity.magnitude);
    }
    void LateUpdate(){
        
    }
    void DetermineState(){
        if(groundTimer > groundDetectionDelay) state = States.walking;
        else state = States.air;
    }
    Vector3 GetWishDir(){
        return (Input.GetAxisRaw("Horizontal") * fppCamera.planarRight + Input.GetAxisRaw("Vertical") * fppCamera.planarForward).normalized;
    }
    Vector3 ApplyGravity(Vector3 vel, float grav){
        return vel - Vector3.up * grav;
    }
    Vector3 Accelerate(Vector3 vel, Vector3 dir, float accel, float cap){
        float speed = Vector3.Dot(Vector3.Scale(plane, vel), dir);
        float speedGain = accel * Time.fixedDeltaTime;
        if(speed + speedGain > cap) speedGain = Mathf.Clamp(cap - speed, 0, cap);
        return vel + dir * speedGain;
    }
    public Vector3 GroundMove(Vector3 dir, Vector3 vel, float accel, float cap){
        Vector3 fricVel = ApplyFriction(vel, drag);
        vel = new Vector3(fricVel.x, fricVel.y, fricVel.z);
        return Accelerate(vel, dir, accel, cap);
    }
    public Vector3 AirMove(Vector3 dir, Vector3 vel, float accel, float cap){
        return Accelerate(vel, dir, accel, cap);
    }
    public Vector3 ApplyFriction(Vector3 vel, float fric){
        return vel * (1 / (fric + 1));
    }
    void OnControllerColliderHit(ControllerColliderHit collision){
        normal = collision.normal;
            float momentum = Vector3.Dot(collision.normal, velocity);
            velocity -= collision.normal * momentum;
            velocity -= normal * adhesion;
    }
}
