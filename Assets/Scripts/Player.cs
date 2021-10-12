using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float RotationSpeed = 1.0f;
    public Vector3 itemPosition;
    public Image MainIcon;
    public Image LeftIcon;
    public Image RightIcon;
    public Texture2D cursor;
    public GameObject zombie;

    private Vector3 _movement;
    private Vector3 _rotation;
    public Camera myCamera;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float HortMouseMove = Input.GetAxis("Mouse X");
        float VertMouseMove = Input.GetAxis("Mouse Y");

        _movement = transform.right * horizontalMove + transform.forward * verticalMove;

        if (_movement != Vector3.zero)
        {
            gameObject.transform.forward = _movement;
        }

        _rotation.x += HortMouseMove * RotationSpeed;
        _rotation.y -= VertMouseMove * RotationSpeed;
        _rotation.y = Mathf.Clamp(_rotation.y, -90f, 90f);
        myCamera.transform.localRotation = Quaternion.Euler(_rotation.y, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, _rotation.x, 0f);
        RaycastHit hit;
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            //Debug.Log("You have hit: " + hit.transform.name);
            if (Input.GetButtonUp("Fire1"))
            {
                hit.transform.gameObject.GetComponentInParent<Zombie>().Damage();
            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            Instantiate(zombie, new Vector3(-1.31f, 1.624f, 34.4f), Quaternion.Euler(new Vector3(0,180,0)));
        }

    }
    private void FixedUpdate()
    {
        transform.Rotate(_rotation * Time.fixedDeltaTime * RotationSpeed);
        controller.Move(_movement * Time.fixedDeltaTime * playerSpeed);
    }
}