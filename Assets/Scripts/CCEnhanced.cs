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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveBuffer = new Vector3(Input.GetAxis("Horizontal"), isStepping ? 0 : -1f, Input.GetAxis("Vertical")) * 15 * Time.deltaTime;
        player.Move(moveBuffer);
        hits = Physics.SphereCastAll(transform.position + Vector3.up * player.height/2, player.radius * 2.5f, Vector3.down, player.height, ~layerMask);
        if(lastHits != null)if(!hits.SequenceEqual(lastHits)){
            change = true;
        }else{
            change = false;
        }
        if(change){
            Hitpoints.Clear();
            foreach(RaycastHit hit in hits){
                Vector3 a = Vector3.Scale(moveBuffer.normalized, new Vector3(1, 0, 1)).normalized;
                Vector3 b = Vector3.Scale(hit.point - transform.position, new Vector3(1, 0, 1)).normalized;
                DrawVector(transform.position, a, 10, Color.yellow);
                DrawVector(transform.position, b, 10, Color.green);
                dot = Vector3.Dot(a, b);
                if(dot > 0.9f){
                    Hitpoints.Add(hit.point);
                    DrawVector(hit.point, Vector3.up, 100, Color.black);
                }
            }
        }

            if(Hitpoints.Count > 0) 
            {   foreach(Vector3 hit1 in Hitpoints){
                dot = Vector3.Dot(Vector3.Cross(moveBuffer.normalized, new Vector3(1, 0, 1)), hit1 - transform.position);
                Vector3 difference = Lerp3D(ref buffer, player.velocity.magnitude*3 * Time.deltaTime);
                transform.position += difference;
            }
                isStepping = true;
            }else{
                isStepping = false;
            }
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
}
