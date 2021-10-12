using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : NPC
{
    [SerializeField] private GameObject target;
    [SerializeField] private int hp = 100;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private int WanderTimer = 10;
    [SerializeField] private Color32 hitColor;
    [SerializeField] private Material normal;
    public RaycastHit hit;

    private string state = "Wander";
    void Start()
    {
        target = GameObject.Find("Target");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case ("Wander"):
                Wander();
                break;
            case ("Walk"):
                Walk();
                break;
            case ("Attack"):
                Attack();
                break;
        }
    }
    void Wander()
    {
        WanderTimer -= 1;
        if (WanderTimer < 0)
        {
            state = "Walk";
        }
    }

    void Walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        if (transform.position == target.transform.position)
        {
            state = "Attack";
        }
    }

    void Attack()
    {

    }
    public void Damage()
    {
        hp -= 50;
        StartCoroutine(DamageReaction());
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DamageReaction()
    {
        //GetComponent<MeshRenderer>().material.color = hitColor;
        yield return new WaitForSeconds(2);
        //GetComponent<MeshRenderer>().material = normal;
        StopAllCoroutines();
    }
}