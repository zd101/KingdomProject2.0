using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameramovescript : MonoBehaviour
{
    public GameObject target;  // Reference to the player's transform

    void Start() {
        
    }

    void Update() {
               transform.position = new Vector3(target.transform.position.x, transform.position.y, -10);

    }
  
}
