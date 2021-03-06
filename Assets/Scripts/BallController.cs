using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {

    public static int score1 = 0;
    public static int score2 = 0;
    public static Rigidbody rb;
    public static bool turn = true;
    public static Text player1ExternText;
    public static Text player2ExternText;
    public static bool playing;
    public static bool changePlayer;
    public static int ballsIn;

	public Text player1Text;
	public Text player2Text;
    public float moveVertical;
    public float speed;
    public Vector3 playerPosition;
    public Vector3 endLine;
    public Vector3 lineSize;
    public LineRenderer line;
    public int maxForce;
    public int minForce;

    void Start()
    {
        speed = 1900;
        maxForce = 6;
        minForce = -4;
        moveVertical = 0;
        playing = true;
        rb = GetComponent<Rigidbody>();
        rb.mass = 6;
        lineSize = new Vector3(10.0f, 0.0f, 0.0f);
        playerPosition = rb.transform.position;
        endLine = playerPosition + lineSize;
        line = GetComponent<LineRenderer>();
        player1Text.text = "Player 1 (full) : 0";
        player1Text.color = Color.red;
        player2Text.text = "Player 2 (half full) : 0";
        player1ExternText = player1Text;
        player2ExternText = player2Text;
        changePlayer = false;
        ballsIn = -1;
    }

    void FixedUpdate()
    {
        if (playing) {
            stopBalls();
            changeTurn();
            playerPosition = rb.transform.position;

            /// Stop if the speed is too low
            if(rb.velocity.magnitude < 2)
            {
                rb.velocity = Vector3.zero;
            }
            
            /// Change the direction
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Quaternion rotate = Quaternion.Euler(0, -2, 0);
                lineSize = rotate * lineSize;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Quaternion rotate = Quaternion.Euler(0, 2, 0);
                lineSize = rotate * lineSize;
            }

            /// Ajust the speed
            endLine = playerPosition + lineSize;
            moveVertical += Input.GetAxis("Vertical");
            
            if (moveVertical >= maxForce)
            {
                moveVertical = maxForce;
            }
            else if(moveVertical <= minForce)
            {
                moveVertical = minForce;
            }

            endLine += lineSize.normalized * moveVertical;
            line.SetPosition(0, playerPosition);
            line.SetPosition(1, endLine);

            if (Input.GetKey(KeyCode.Space) && !isMoving())
            {
                changePlayer = false;
                ballsIn = 0;
                rb.AddForce((endLine - playerPosition) * speed);
            }
            if (Input.GetKey(KeyCode.RightControl))
            {
                rb.transform.position = new Vector3(-23f, 25.95059f, 0f);
            }

            
        }
    }

    /// Return true if any of the balls is moving.
    bool isMoving()
    {
        bool response = false;
        for(int i = 0; i<16; ++i)
        {
            GameObject go = GameObject.Find($"Ball ({i})");
            if(go is null) continue;

            Rigidbody rb = go.GetComponent<Rigidbody>();
            
            if(rb.velocity.magnitude > 0)
            {
                response = true;
            }
        }

        return response;
    }

    /// Stop all balls moving if the speed is too low
    void stopBalls()
    {
        for(int i = 0; i<16; ++i)
        {
            GameObject go = GameObject.Find($"Ball ({i})");
            if(go is null) continue;

            Rigidbody rb = go.GetComponent<Rigidbody>();
            
            if(rb.velocity.magnitude < 3)
            {
                rb.velocity = Vector3.zero;
                
            }
            if(rb.angularVelocity.magnitude < 2)
            {
                rb.angularVelocity = Vector3.zero;      
            }
        }
        
    }

    /// Change the [turn] if is the case.
    void changeTurn()
    {
        if(!isMoving())
        {
            if(changePlayer || ballsIn == 0)
            {
                turn = !turn;
                changePlayer = false;
                ballsIn = -1;
                if(turn) Debug.Log("Player changed: 1");
                else Debug.Log("Player changed: 2");
            }
            if (turn)
            {
                player1Text.color = Color.red;
                player2Text.color = Color.white;
            }
            else
            {
                player2Text.color = Color.red;
                player1Text.color = Color.white;
            }
        }
    }
}
