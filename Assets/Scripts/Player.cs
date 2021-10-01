using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public List<PickupItem> inventory;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float RotationSpeed = 1.0f;
    public PickupItem OldItem;
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

        if (Input.mouseScrollDelta.y > 0f)
        {
            //Scrolling Up.
            InvPos--;
            if (InvPos < 0)
            {
                InvPos = Length();
            }
            DrawItem(inventory[InvPos]);
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            //Scrolling Down.
            InvPos++;
            if (InvPos > Length())
            {
                InvPos = 0;
            }
            DrawItem(inventory[InvPos]);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Weapon tempWeapon = (Weapon)GetDrawnItem();
            tempWeapon.Swing();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, myCamera.transform.forward, out hit, 3f))
            {
                Debug.Log("You have hit: " + hit.transform.name);
                Use(hit.transform.gameObject.GetComponentInParent<GameEntity>());
            }
        }
    }
    private void FixedUpdate()
    {
        transform.Rotate(_rotation * Time.fixedDeltaTime * RotationSpeed);
        controller.Move(_movement * Time.fixedDeltaTime * playerSpeed);
    }

    private void Use(GameEntity go)
    {
        go.Use(this);
    }
    public void DrawItem(PickupItem Item)
    {
        ResetIcons();
        if (OldItem != null)
        {
            OldItem.gameObject.SetActive(false);
        }
        MainIcon.sprite = Item.itemSprite;
        Item.gameObject.SetActive(true);
        OldItem = Item;
    }

    public void ResetIcons()
    {
        if (inventory.Count > 1)
        {
            if (InvPos == Length())
            {
                LeftIcon.sprite = inventory[0].itemSprite;
                RightIcon.sprite = inventory[InvPos - 1].itemSprite;
                //Debug.Log("InvPos == Length()");
            }
            else if (InvPos == 0)
            {
                LeftIcon.sprite = inventory[InvPos + 1].itemSprite;
                RightIcon.sprite = inventory[Length()].itemSprite;
                //Debug.Log("InvPos == 0");
            }
            else
            {
                LeftIcon.sprite = inventory[InvPos + 1].itemSprite;
                RightIcon.sprite = inventory[InvPos - 1].itemSprite;
                //Debug.Log("else");
            }
        }
        else
        {
            LeftIcon.sprite = inventory[0].itemSprite;
            RightIcon.sprite = inventory[0].itemSprite;
            MainIcon.sprite = inventory[0].itemSprite;
        }
    }

    public PickupItem GetDrawnItem()
    {
        return inventory[InvPos];
    }

    private int Length()
    {
        if (inventory.Count > 0)
        {
            return inventory.Count - 1;
        }
        return inventory.Count;
    }
}