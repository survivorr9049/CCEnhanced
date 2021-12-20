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
    bool isStepping;
    public Vector3 moveBuffer;
    public float dot;
    RaycastHit[] lastHits;
    RaycastHit[] hits;
    bool change;
    public float stepOffset;
    public float scanRadius;
    public float scanForwardsStep;
    public GameObject cube;
    Vector3 lastMove;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
       Move(new Vector3(Input.GetAxis("Horizontal"), -1f, Input.GetAxis("Vertical")) * 5 * Time.deltaTime);
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
        if(lastHits != null)if(!hits.SequenceEqual(lastHits)){
            change = true;
        }else{
            change = false;
        }
        if(change && moveBuffer.magnitude > 0){
            Hitpoints.Clear();
            foreach(RaycastHit hit in hits){
                Vector3 a = Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized;
                Vector3 b = Vector3.Scale(hit.point - transform.position, new Vector3(1, 0, 1)).normalized;
                DrawVector(transform.position, a, 10, Color.yellow);
                DrawVector(transform.position, b, 10, Color.green);
                DrawVector(hit.point, Vector3.up, 10, Color.cyan);
                dot = Vector3.Dot(a, b);
                if(dot > 0.9f && (1-(transform.position.y - hit.point.y)) <= stepOffset && (1-(transform.position.y - hit.point.y)) > 0){
                    Hitpoints.Add(hit.point);
                    DrawVector(hit.point, Vector3.up, 100, Color.black);
                }
            }
        }
            if(Hitpoints.Count > 0) 
            {   
                foreach(Vector3 hit1 in Hitpoints){
                    dot = Vector3.Dot(Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized, Vector3.Scale(hit1 - transform.position, new Vector3(1, 0, 1)).normalized);
                    buffer = new Vector3(0, (1-(transform.position.y - hit1.y)), 0);
                    Vector3 difference = Lerp3D(ref buffer, player.velocity.magnitude*3 * Time.deltaTime);
                    transform.position += difference;
                }
                isStepping = true;
            }else{
                isStepping = false;
            }
    }
}
