using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        moveTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveTarget()
    {
        transform.position = new Vector3(Random.Range(-15, 15), 0.5f, Random.Range(-15, 15));
    }
}
