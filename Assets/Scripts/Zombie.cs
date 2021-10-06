using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : NPC
{
    public float enemySpeed;
    public float enemyRotSpeed;
    public RaycastHit hit;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        controller.Move(transform.forward * Time.fixedDeltaTime * enemySpeed);
    }
}
