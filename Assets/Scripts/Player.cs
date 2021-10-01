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

    private Vector3 _movement;
    private Vector3 _rotation;
    public GameObject myCamera;
    private int InvPos;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        myCamera = GameObject.Find("Main Camera");
        InvPos = 0;
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

    }
    private void FixedUpdate()
    {
        transform.Rotate(_rotation * Time.fixedDeltaTime * RotationSpeed);
        controller.Move(_movement * Time.fixedDeltaTime * playerSpeed);
    }
}