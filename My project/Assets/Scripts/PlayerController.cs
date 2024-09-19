using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;

    Vector2 camRotation;

    [Header("Player Stats")]
    public int health = 5;
    public int maxHealth = 10;
    public int healtPickupAmt = 5;

    [Header("Weapon Stats")]
    public Transform weaponSlot;

    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float sprintMult = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetection = 1f;

    [Header("User Settings")]
    public bool sprintToggle = false;
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float camRotationLimit = 90f;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        playerCam = transform.GetChild(0).GetComponent<Camera>();

        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        camRotation.y = Mathf.Clamp(camRotation.y, -90, 90);

        playerCam.transform.localRotation = Quaternion.AngleAxis(camRotation.y, Vector3.left); 
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

        sprinting = (((!sprinting) && (!sprintToggle && Input.GetKey(KeyCode.LeftShift)) || (sprintToggle && Input.GetAxisRaw("Vertical") <= 0)));

        Vector3 temp = myRB.velocity;

        temp.x = Input.GetAxisRaw("Horizontal") * speed;
        temp.z = Input.GetAxisRaw("Vertical") * speed;

        if (sprinting)
            temp.z *= sprintMult;

        if (sprinting && sprintToggle && (Input.GetAxisRaw("Vertical") <= 0))
            sprinting = false;

        if (sprinting && Input.GetKeyUp(KeyCode.LeftShift))
            sprinting = false;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
            temp.y = jumpHeight;

        myRB.velocity = (transform.forward * temp.z) + (transform.right * temp.x) + (transform.up * temp.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.tag == "healthPickup") && health < maxHealth)
        {
            if (health + healtPickupAmt > maxHealth)
                health = maxHealth;

            else
                health += healtPickupAmt;

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "weapon")
        {
            other.transform.SetPositionAndRotation(weaponSlot.position, weaponSlot.rotation);

            other.transform.SetParent(weaponSlot);
        }
    }
}
