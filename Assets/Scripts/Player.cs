using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f;
    public float RotationSpeed = 1.0f;
    public Texture2D cursor;
    public GameObject zombie;
    public GameObject UI;
    public bool gameStart = false;
    private int gameStarter = 30;
    
    public Text healthText;
    public Text ScoreText;
    public int maxhealth = 3;
    public int health;

    public int Score = 0;

    [SerializeField] private Text textAmmo;
    public int maxAmmo = 8;
    public int ammo;
    public int reloadTime = 15;
    public bool reloading = false;
    public AudioSource gunShot;
    public AudioSource reload;

    private Vector3 _movement;
    private Vector3 _rotation;
    public Camera myCamera;

    private void Start()
    {
        Score = 0;
        health = maxhealth;
        ammo = maxAmmo;
        textAmmo.text = ammo + " / " + maxAmmo;
        healthText.text = health + " ";
        ScoreText.text = "Score: "+ Score;

        AudioSource[] allMyAudioSources = GetComponents<AudioSource>();
        gunShot = allMyAudioSources[0];
        reload = allMyAudioSources[1];
        

        controller = gameObject.GetComponent<CharacterController>();
        Vector2 hotSpot = new Vector2(cursor.width / 2f, cursor.height / 2f);
        Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        textAmmo.text = ammo + " / " + maxAmmo;

        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float HortMouseMove = Input.GetAxis("Mouse X");
        float VertMouseMove = Input.GetAxis("Mouse Y");

        _movement = transform.right * horizontalMove + transform.forward * verticalMove;

        if (_movement != Vector3.zero)
        {
            gameObject.transform.forward = _movement;
        }

        //_rotation.x += HortMouseMove * RotationSpeed;
        //_rotation.y -= VertMouseMove * RotationSpeed;
        _rotation.y = Mathf.Clamp(_rotation.y, -90f, 90f);
        myCamera.transform.localRotation = Quaternion.Euler(_rotation.y, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, _rotation.x, 0f);
        RaycastHit hit;
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

        if (reloading == true){
            reloadTime -= 1;
            if (reloadTime == 14){
                reload.Play();
            } 
            if (reloadTime <= 0){
                Reload();
                reloadTime = 15;
                reloading = false;
            }
        }
        
        //Debug.Log("You have hit: " + hit.transform.name);
        //Shoot, play sound, and reload.
        if (gameStart == true){
            gameStarter -= 1;
            if (gameStarter <= 0){
                if (Input.GetButtonUp("Fire1")){
                    if (ammo >= 1){
                        ammo -= 1;
                        gunShot.Play();
                        if (Physics.Raycast(ray, out hit, 100f))
                        {
                            if (hit.transform.CompareTag("Zombie"))
                            {
                                hit.transform.gameObject.GetComponentInParent<Zombie>().Damage();
                            }
                        }
                    } else {
                        reloading = true;
                    }
                }
            }
        }

    }

    public void Reload()
    {
        ammo = maxAmmo;
    }

    public void Dmg()
    {
        health -= 1;
        healthText.text = health + " ";
        if (health <= 0){
            Death();
        }
    }

    public void Death()
    {
        Time.timeScale = 0f;
        UI.GetComponentInParent<Menu>().ScoreText.text = Score.ToString();
        UI.SetActive(true);
    }

    private void FixedUpdate()
    {
        //transform.Rotate(_rotation * Time.fixedDeltaTime * RotationSpeed);
        //controller.Move(_movement * Time.fixedDeltaTime * playerSpeed);
    }
}