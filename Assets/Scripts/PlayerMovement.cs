using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private static Quaternion workingQuaternion = new Quaternion();
    public float speed = 6.0f;

    private Vector3 movement;
    //private Animator anim;
    private int floorMask;
    private float camRayLength = 100.0f;
    private Rigidbody playerRigidbody;
    public Vector3 offset = new Vector3(0.0f, 6.0f, -8.0f);
    public Joystick joyStick;

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
    }

    void FixedUpdate()
    {
        float h;
        float v;
        if (Application.isEditor)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
        else
        {
            h = this.joyStick.position.x;
            v = this.joyStick.position.y;
        }

        //rotation
        this.transform.rotation = (Quaternion.AngleAxis(Mathf.Atan2(h, v) * Mathf.Rad2Deg, Vector3.up));

        //movement
        if (h != 0 || v != 0)
            this.transform.Translate(transform.forward * this.speed * Time.fixedDeltaTime, Space.World);
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
