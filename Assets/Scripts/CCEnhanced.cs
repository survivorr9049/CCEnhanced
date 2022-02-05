using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CCEnhanced : MonoBehaviour
{
    List<Vector3> Hitpoints = new List<Vector3>();
    [HideInInspector] public CharacterController player;
    public LayerMask layerMask;
    public float height;
    public float radius;
    public bool isGrounded;
    Vector3 buffer;
    Vector3 groundNormal;
    RaycastHit groundDetection;
    RaycastHit stepDetection;
    RaycastHit hitLast;
    RaycastHit[] hits;
    public bool isStepping;
    Vector3 moveBuffer;
    float dot;
    [Range(0.0f, 2.0f)]
    [SerializeField]float stepOffset;
    [Range(0.0f, 4.0f)]
    [SerializeField]float scanRadius;
    [Range(0.0f, 4.0f)]
    [SerializeField]float scanForwardsStep;
    [Range(0.0f, 15.0f)]
    [SerializeField]float ascendSpeed;
    [SerializeField]float ascendMargin;
    [SerializeField]float groundDetectionMargin;
    [SerializeField]float groundDetectionSphereScale;
    bool canStep;
    bool isStair;
    Vector3 playerToPoint;
    bool breaks;
    Vector3[] controllerTip;
    Collider[] collisions;
    CapsuleCollider playerCollider;
    void Start(){
        if(!GetComponent<CharacterController>()){
            player = gameObject.AddComponent<CharacterController>();
        }else{
            player = GetComponent<CharacterController>();
        }
        player.stepOffset = 0.1f;
        player.slopeLimit = 0;
        player.height = height;
        player.radius = radius;
        player.skinWidth = 0.001f;
    }
    void Update(){
        if(!isStepping)isGrounded = CheckGrounded();
    }
    public Vector3 Lerp3D(ref Vector3 buffer, float speed){
        Vector3 difference = buffer - Vector3.Lerp(buffer, Vector3.zero, speed);
        buffer -= difference;
        return difference;
    }
    public void Move(Vector3 move){
        if(!isGrounded){
            player.Move(move);
            isStepping = false;
            return;
        }
        float moveY = move.y;
        if(breaks){isStepping = false; breaks = false;}
        if(isStepping)if(move.y < 0)moveY = 0;
        moveBuffer = new Vector3(move.x, moveY, move.z);
        player.Move(moveBuffer);
        hits = Physics.CapsuleCastAll(transform.position + Vector3.up * player.height/2 - new Vector3(move.x, 0, move.z).normalized * scanForwardsStep, transform.position + Vector3.up * player.height/2 + new Vector3(move.x, 0, move.z).normalized * scanForwardsStep, player.radius * scanRadius, Vector3.down, player.height*2, ~layerMask);
        Physics.SphereCast(transform.position + Vector3.up * player.height/2 + playerToPoint.normalized * scanForwardsStep, player.radius, Vector3.down, out hitLast, player.height*2, ~layerMask);
        Physics.SphereCast(transform.position, player.radius * 0.2f, Vector3.down, out groundDetection, player.height + stepOffset, ~layerMask);  
        groundNormal = groundDetection.normal;
        Vector3 slopeDetector = hitLast.point - groundDetection.point;
        Physics.Raycast(groundDetection.point, hitLast.point - groundDetection.point, out stepDetection, 10000, ~layerMask);
        if(Mathf.Abs(Vector3.Dot(stepDetection.normal, hitLast.normal)) < 0.9f) isStair = true;
        else isStair = false;
        if(Vector3.Dot(groundNormal, slopeDetector) < 0.05f && Vector3.Dot(groundNormal, slopeDetector) > -0.05f) canStep = false;
        else canStep = true;
        if(moveBuffer.magnitude > 0){
            Hitpoints.Clear();
            if(Vector3.Dot(playerToPoint.normalized, Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized) > -0.05f && transform.position.y-1 <= hitLast.point.y)Hitpoints.Add(hitLast.point);
            foreach(RaycastHit hit in hits){
                Vector3 a = Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized;
                Vector3 b = Vector3.Scale(hit.point - transform.position, new Vector3(1, 0, 1)).normalized;
                dot = Vector3.Dot(a, b);
                if(dot > 0.9f && (1-(transform.position.y - hit.point.y)) <= stepOffset && (1-(transform.position.y - hit.point.y)) > 0 && transform.position.y-1 <= hit.point.y){
                    Hitpoints.Add(hit.point);
                }
            }
        }
            if(Hitpoints.Count > 0) {
                foreach(Vector3 hit1 in Hitpoints) playerToPoint = Vector3.Scale(new Vector3(1, 0, 1), hit1 - transform.position);
                if(canStep && isStair){
                    foreach(Vector3 hit1 in Hitpoints){
                        dot = Vector3.Dot(Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized, Vector3.Scale(hit1 - transform.position, new Vector3(1, 0, 1)).normalized);
                        if(dot > 0.9f && transform.position.y-1 <= hit1.y){
                            buffer = new Vector3(0, (1-(transform.position.y - hit1.y)) + ascendMargin, 0);
                            Vector3 difference = Lerp3D(ref buffer, player.velocity.magnitude* ascendSpeed * Time.deltaTime);
                            transform.position += difference;
                        }                    
                    }
                    isStepping = true;
                }
            }else{
                isStepping = false;
            }
    } 
    public bool CheckGrounded(){
        Vector3 tip = transform.position + player.center - Vector3.up*player.height/2+Vector3.up*player.radius-Vector3.up*groundDetectionMargin;
        return Physics.CheckSphere(tip, player.radius * groundDetectionSphereScale, ~layerMask, QueryTriggerInteraction.Ignore);
    }
    ///checks for obstacles above player
    public bool CheckHead(){
        Vector3 tip = transform.position + player.center + Vector3.up * player.height/2;
        return !Physics.CheckSphere(tip, player.radius * groundDetectionSphereScale, ~layerMask, QueryTriggerInteraction.Ignore);
    }
    public void SetHeight(float newHeight){
        player.height = newHeight;
        player.center = Vector3.up * (height-newHeight)/2;
    }
}
