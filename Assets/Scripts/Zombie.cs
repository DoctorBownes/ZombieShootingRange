using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private GameObject target;
    [SerializeField] float hp = 100f;
    private float speed = 3f;
    private float attackSpeed = 30f;
    private float wanderTimer = 3f;
    private float scoreMultiply = 0.005f;
    [SerializeField] int scorePoints = 10;
    [SerializeField] private Color32 hitColor;
    [SerializeField] private Material normal;
    [SerializeField] private GameObject deathParticles;
    public Player player;
    public RaycastHit hit;

    private float aSpeed;

    private string state = "Wander";
    void Start()
    {
        speed += Random.Range(-2f, 2f);
        aSpeed = attackSpeed;
        target = GameObject.Find("Target");
        player = GameObject.Find("Player").GetComponent<Player>();
        transform.LookAt(target.transform);

        hp += player.Score * scoreMultiply;
        speed += player.Score * scoreMultiply;

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
        wanderTimer -= 1;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, -speed * Time.deltaTime);
        if (wanderTimer < 0)
        {
            state = "Walk";
        }
    }

    void Walk()
    {   
        
        Ray ray = new Ray(transform.position, transform.forward);
        LayerMask mask = LayerMask.GetMask("Zombie");
        if (Physics.Raycast(ray, 0.5f, mask)){
            wanderTimer = Random.Range(3, 10);
            state = "Wander";
        } else{
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (transform.position == target.transform.position)
            {           
                state = "Attack";
            }
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
        hp -= 5f;
        // StartCoroutine(DamageReaction());
        if (hp <= 0)
        {
            player.Score += scorePoints;
            player.ScoreText.text = "Score: " + player.Score;
            Destroy(Instantiate(deathParticles, transform.position, Quaternion.identity) as GameObject, 2f);
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