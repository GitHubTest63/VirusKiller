using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private static Quaternion workingQuaternion = new Quaternion();
    public float speed = 6.0f;
    public float rotationSpeed = 1.0f;

    private Vector3 movement;
    //private Animator anim;
    private int floorMask;
    private float camRayLength = 100.0f;
    private Rigidbody playerRigidbody;
    public Vector3 offset = new Vector3(0.0f, 6.0f, -8.0f);
    public Joystick joyStick;
    public bool editorMode;
    private Quaternion targetRotation;
    private bool canRotate = false;

    void Start()
    {
        if (this.joyStick == null)
        {
            GameObject joystickGO = GameObject.FindGameObjectWithTag("Joystick");
            this.joyStick = joystickGO.GetComponent<Joystick>();
        }
        this.floorMask = LayerMask.GetMask("Floor");
        //anim = GetComponent<Animator>();
        this.playerRigidbody = GetComponent<Rigidbody>();
        this.targetRotation = this.transform.rotation;
    }

    void FixedUpdate()
    {
        float h;
        float v;
        if (this.editorMode)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
        else
        {
            h = this.joyStick.position.x;
            v = this.joyStick.position.y;
        }

        if (h != 0 || v != 0)
        {
            //rotation
            this.targetRotation = (Quaternion.AngleAxis(Mathf.Atan2(h, v) * Mathf.Rad2Deg, Vector3.up));
            this.canRotate = true;
            //movement
            this.transform.Translate(transform.forward * this.speed * Time.fixedDeltaTime, Space.World);
            //NetworkManager.Instance.send("PositionMessage", this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }

        if (this.canRotate)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, this.targetRotation, this.rotationSpeed * Time.fixedDeltaTime);
            if (Mathf.Abs(this.targetRotation.x - this.transform.rotation.x) <= Quaternion.kEpsilon)
            {
                this.canRotate = false;
                this.transform.rotation = this.targetRotation;
            }
        }
    }

    private void updateRotation()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0.0f;

            workingQuaternion.SetLookRotation(playerToMouse);
            playerRigidbody.MoveRotation(workingQuaternion);
        }
    }
}
