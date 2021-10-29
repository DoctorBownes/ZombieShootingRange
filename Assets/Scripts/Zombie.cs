using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private int hp = 100;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private int attackSpeed = 100;
    [SerializeField] private int WanderTimer = 10;
    [SerializeField] private Color32 hitColor;
    [SerializeField] private Material normal;
    [SerializeField] public Player player;
    public RaycastHit hit;

    private int aSpeed;

    private string state = "Wander";
    void Start()
    {
        aSpeed = attackSpeed;
        target = GameObject.Find("Target");
        player = GameObject.Find("Player").GetComponent<Player>();
        transform.LookAt(target.transform);
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
        aSpeed -= 1;
        if (aSpeed <= 0){
            player.GetComponent<Player>().Dmg();
            aSpeed = attackSpeed;
        }
    }

    public void Damage()
    {
        hp -= 50;
        // StartCoroutine(DamageReaction());
        if (hp <= 0)
        {
            player.Score += 10;
            player.ScoreText.text = "Score: " + player.Score;
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