using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    //pole sprint and stamina
    public Slider stamina;
    public float recoveryRate = 0.5f;
    public float fatigueRate = 0.2f;
    public float sprintMultiplier = 1.3f;
    //pole movement
    public float speed = 5.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    //pole sneak
    public bool isCrouching = false;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 3.5f;
    public float crouchTransitionSpeed = 10f;
    private float originalHeight;
    public float crouchCameraOffset = -0.5f;
    private Vector3 cameraStandPosition;
    private Vector3 cameraCrouchPosition;
    //pole rotate head
    public Transform head;
    public float xAngleOffset = 0.3f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;
        //stamina = GetComponent<Slider>();
        originalHeight = characterController.height;

        // Define camera positions for standing and crouching
        cameraStandPosition = playerCamera.transform.localPosition;
        cameraCrouchPosition = cameraStandPosition + new Vector3(0, crouchCameraOffset, 0);

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        //Sneak
        if (Input.GetKeyDown(KeyCode.C) && canMove)
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                speed = crouchSpeed;
            }
            else
            {
                characterController.height = originalHeight;
                speed = 5.5f;
            }
        }
        if (isCrouching)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraCrouchPosition, crouchTransitionSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraStandPosition, crouchTransitionSpeed * Time.deltaTime);
        }


        //Call Methods

        if (characterController.isGrounded)
        {

            PlayerSprint();
        }
        PlayerMoveForward();
        //  PlayerSneak();

    }
    void PlayerSprint()
    {
        //sprint and stamina
        if (stamina.value > 5 && Input.GetButton("Sprint"))
        {
            characterController.Move(moveDirection * sprintMultiplier * Time.deltaTime);
            stamina.value -= fatigueRate;
        }
        if (stamina.value < 100 && Input.GetButton("Sprint") != true)
        {
            stamina.value += recoveryRate;
        }
    }
    //void PlayerSneak()
    // {
    //if (Input.GetButton("Sneak"))
    // {
    // characterController.height = 1f;
    //  Debug.Log(1488);
    // }
    //if (Input.GetKeyDown(KeyCode.LeftControl))
    //{
    //characterController.height = 1f;
    //gravityPlayer.groundCheck.transform.localPosition = new Vector3(0, -0.5f, 0);
    //}
    //if (Input.GetKeyUp(KeyCode.LeftControl))
    // {
    //characterController.height = 2f;
    //gravityPlayer.groundCheck.transform.localPosition = new Vector3(0, -1f, 0);
    //  }
    //}

    void PlayerMoveForward()
    {
        if (characterController.isGrounded)
        {
            // isGrounded 
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = speed * Input.GetAxis("Vertical");
            float curSpeedY = speed * Input.GetAxis("Horizontal");
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            //rotate head
            if (Input.GetKey(KeyCode.Q))
            {
                head.transform.localPosition = new Vector3(-xAngleOffset, head.localPosition.y, head.localPosition.z);
                playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 20f);
                //playerCamera.transform.localRotation = Quaternion.Euler(head.localRotation.x, head.localRotation.y, 20f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                head.transform.localPosition = new Vector3(xAngleOffset, head.localPosition.y, head.localPosition.z);
                playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, -20f);
                //playerCamera.transform.localRotation = Quaternion.Euler(head.localRotation.x, head.localRotation.y, -20f);
            }
            else
            {
                //head.transform.localPosition = new Vector3(0, head.localPosition.y, head.localPosition.z);
                playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            }
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}
