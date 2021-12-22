using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CCEnhanced : MonoBehaviour
{
    public List<Vector3> Hitpoints = new List<Vector3>();
    public CharacterController player;
    public LayerMask layerMask;
    Vector3 buffer;
    public Vector3 groundNormal;
    RaycastHit groundDetection;
    RaycastHit stepDetection;
    bool isStepping;
    public Vector3 moveBuffer;
    public float dot;
    RaycastHit[] lastHits;
    RaycastHit hitLast;
    RaycastHit[] hits;
    bool change;
    public float stepOffset;
    public float scanRadius;
    public float scanForwardsStep;
    public float ascendSpeed;
    public bool canStep;
    public bool isStair;
    public GameObject cube;
    public Vector3 playerToPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
       Move(new Vector3(Input.GetAxis("Horizontal"), -1f, Input.GetAxis("Vertical")) * 5 * Time.deltaTime);
       DrawVector(transform.position, playerToPoint, 1, Color.blue);
    }
    void LateUpdate(){
        lastHits = hits;
    }
    public void DrawVector(Vector3 origin, Vector3 vector, float lengthMultiplier, Color color){
        Debug.DrawLine(origin, origin + vector * lengthMultiplier, color);
    }
    public Vector3 Lerp3D(ref Vector3 buffer, float speed){
        Vector3 difference = buffer - Vector3.Lerp(buffer, Vector3.zero, speed);
        buffer -= difference;
        return difference;
    }
    public void Move(Vector3 move){
        float moveY = move.y;
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
        DrawVector(hitLast.point + Vector3.right * 0.01f, hitLast.normal, 10, Color.magenta);
        DrawVector(stepDetection.point, stepDetection.normal, 10, Color.red);
        if(Vector3.Dot(groundNormal, slopeDetector) < 0.05f && Vector3.Dot(groundNormal, slopeDetector) > -0.05f) canStep = false;
        else canStep = true;
        if(lastHits != null)if(!hits.SequenceEqual(lastHits)){
            change = true;
        }else{
            change = false;
        }
        if(change && moveBuffer.magnitude > 0){
            Hitpoints.Clear();
            if(Vector3.Dot(playerToPoint.normalized, Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized) > -0.05f)Hitpoints.Add(hitLast.point);
            foreach(RaycastHit hit in hits){
                Vector3 a = Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized;
                Vector3 b = Vector3.Scale(hit.point - transform.position, new Vector3(1, 0, 1)).normalized;
                DrawVector(transform.position, a, 10, Color.yellow);
                DrawVector(transform.position, b, 10, Color.green); 
                DrawVector(hit.point, hit.normal, 10, Color.cyan);
                dot = Vector3.Dot(a, b);
                if(dot > 0.9f && (1-(transform.position.y - hit.point.y)) <= stepOffset && (1-(transform.position.y - hit.point.y)) > 0){
                    Hitpoints.Add(hit.point);
                    DrawVector(hit.point, Vector3.up, 100, Color.black);
                }
            }
        }
            if(Hitpoints.Count > 0){
                foreach(Vector3 hit1 in Hitpoints) playerToPoint = Vector3.Scale(new Vector3(1, 0, 1), hit1 - transform.position);
            }
            if(Hitpoints.Count > 0 && canStep && isStair) 
            {   
                foreach(Vector3 hit1 in Hitpoints){
                    dot = Vector3.Dot(Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized, Vector3.Scale(hit1 - transform.position, new Vector3(1, 0, 1)).normalized);
                    if(dot > 0.9f){
                        buffer = new Vector3(0, (1-(transform.position.y - hit1.y)), 0);
                        Vector3 difference = Lerp3D(ref buffer, player.velocity.magnitude* ascendSpeed * Time.deltaTime);
                        transform.position += difference;
                    }                    
                }
                isStepping = true;
            }else{
                isStepping = false;
            }
    }
}
