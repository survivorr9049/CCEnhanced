using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CCnormal : MonoBehaviour
{
    public CharacterController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveBuffer = new Vector3(Input.GetAxis("Horizontal"), -1f, Input.GetAxis("Vertical")) * 3 * Time.deltaTime;
        player.Move(moveBuffer);
    }

}
