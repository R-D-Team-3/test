using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    // Start is called before the first frame update
    float rubber_strain;
    float rubber_force;
    Touch pulltouch;
    Vector2 pull_pos;
    Vector2 start_pos;

    void Start()
    {
        
        rubber_strain = 0f;
        rubber_force = 1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 )
        {
            pulltouch = Input.GetTouch(0); 
            start_pos = pulltouch.rawPosition;
            if((start_pos.x > Screen.width/3) && (start_pos.x < 2*Screen.width/3) && (start_pos.y > 3*Screen.height/4 ))
            {
                pull_pos = pulltouch.position;
                rubber_strain = (pull_pos.y - start_pos.y)*400/Screen.height;
            }
        }
        else
        {
            if(rubber_strain > 0f)
            {
                rubber_strain=0f;
            }
        }
        transform.localScale = new Vector3(1,1,1+(rubber_strain*15/100));
    }
}
