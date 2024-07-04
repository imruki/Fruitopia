using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

//wall Sliding Bug ki naguez yokood lesak lhit
//Particle System Jump bug
//KnockBack go down bug and add a bit of random rotation

public class RukiCharacterController : MonoBehaviour
{

    [Header("For Movement")]
    [SerializeField] float playerSpeed;
    [SerializeField] float airMoveSpeed;
    private float XDirectionalInput;
    private bool facingRight = true;

    [Header("For Jumping")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    private int JumpsLeft;
    private float vSpeed;
    private bool isGrounded;
    private bool canJump;

    [Header("For WallSliding")]
    [SerializeField] float wallSlideSpeed;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] Vector2 wallCheckSize;
    private bool isTouchingWall;
    private bool isWallSliding;

    [Header("For WallJumping")]
    [SerializeField] float walljumpforce;
    [SerializeField] Vector2 walljumpAngle;
    [SerializeField] float walljumpDirection = -1;
    private bool isWallJumping;

    [Header("Audio")]
    [SerializeField] private AudioSource LoseAudio;
    [SerializeField] private AudioSource WinAudio;
    [SerializeField] private AudioSource JumpAudio;
    [SerializeField] private AudioSource PayAudio;

    [Header("Other")]
    private Rigidbody2D rb;
    private Animator anim;
    private bool Dead;
    [SerializeField] private CinemachineVirtualCamera myCamera;
    [SerializeField] private float KnockbackForce;
    [SerializeField] private GameObject Transition;
    [SerializeField] private Transform Center;
    [SerializeField] private RuntimeAnimatorController[] Characters;
    [SerializeField] private ParticleSystem RunningDust;
    [SerializeField] private ParticleSystem.VelocityOverLifetimeModule RunningDustVelocity;
    [SerializeField] private ParticleSystem JumpingDust;
    static public int ChosenCharacter;

    void Awake()
    {
        // Make the game run as fast as possible in Windows
        Application.targetFrameRate = 300;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        RunningDustVelocity = RunningDust.velocityOverLifetime;
        anim.runtimeAnimatorController = Characters[ChosenCharacter];
        isGrounded = true;
        JumpsLeft = 2;
        Dead = false;
        facingRight = true;
        isWallJumping = false;
        walljumpAngle.Normalize();

    }

    void Update()
    {
        if (Dead) return;
        CheckWorld();
        Inputs();
        WallJump();
        Jump();
    }
    private void FixedUpdate()
    {
        if (Dead) return;
        Movement();
        WallSlide();
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Ground"))
        {

            isGrounded = true;
            JumpsLeft = 2;
            //canJump = true;
            isWallJumping = false;
        }                  
        if (other.gameObject.CompareTag("Enemy"))
        {
            LoseAudio.Play();
            anim.SetTrigger("Hit");
            GetComponent<BoxCollider2D>().enabled = false;
            Dead = true;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, KnockbackForce));
            myCamera.Follow = null;
            rb.constraints = RigidbodyConstraints2D.None;
            Instantiate(Transition, Center.position, Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(1.5f, SceneManager.GetActiveScene().name));
        }
        if (other.gameObject.CompareTag("finish"))
        {
            WinAudio.Play();
            isGrounded = true;
            JumpsLeft = 2;
            //canJump = true;
            isWallJumping = false;
        }


    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Level"))
        {
            Instantiate(Transition,transform.position,Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(0.7f, other.gameObject.name));
        }
        if (other.gameObject.CompareTag("Finish"))
        {
            WinAudio.Play();
            Instantiate(Transition, Center.position, Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(0.7f, "Lobby"));
        }
        
        if (other.gameObject.CompareTag("Character"))
        {
            if (other.gameObject.name == "Froggy" && CoinManager.coins >= 2 && anim.runtimeAnimatorController != Characters[0])
            {
                PayAudio.Play();
                CoinManager.coins -= 2;
                anim.runtimeAnimatorController = Characters[0];
                ChosenCharacter = 0;
            }
            else if (other.gameObject.name == "PinkMan" && CoinManager.coins >= 7 && anim.runtimeAnimatorController != Characters[1])
            {
                PayAudio.Play();
                CoinManager.coins -= 7;
                anim.runtimeAnimatorController = Characters[1];
                ChosenCharacter = 1;
            }
            else if (other.gameObject.name == "VirtualGuy" && CoinManager.coins >= 5 && anim.runtimeAnimatorController != Characters[2])
            {
                PayAudio.Play();
                CoinManager.coins -= 5;
                anim.runtimeAnimatorController = Characters[2];
                ChosenCharacter = 2;
            }
            else if (other.gameObject.name == "MaskShadow" && CoinManager.coins >= 10 && anim.runtimeAnimatorController != Characters[3])
            {
                PayAudio.Play();
                CoinManager.coins -= 10;
                anim.runtimeAnimatorController = Characters[3];
                ChosenCharacter = 3;
            }

        }
    }

    private void Inputs()
    {
        XDirectionalInput = Input.GetAxis("Horizontal");
    }

    private void Movement()
    {
        vSpeed = rb.velocity.y;
        anim.SetFloat("vSpeed", vSpeed);
        Flip(XDirectionalInput);
        if (isGrounded)
        {
            rb.velocity = new Vector2(XDirectionalInput * playerSpeed, rb.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(XDirectionalInput));
        }
        else if (!isGrounded && (!isWallSliding || !isTouchingWall) && XDirectionalInput != 0)
        {
            rb.AddForce(new Vector2(airMoveSpeed * XDirectionalInput, 0));
            if (Mathf.Abs(rb.velocity.x) > playerSpeed)
            {
                rb.velocity = new Vector2(XDirectionalInput * playerSpeed, rb.velocity.y);
            }
        }
        if (isGrounded && (XDirectionalInput > 0.01 || XDirectionalInput < -0.01)) {
            if (!RunningDust.isPlaying) RunningDust.Play();
        }
        else
        {
            if (RunningDust.isPlaying) RunningDust.Stop();


        }
    }



    private void Jump()
    {
        if (isGrounded) {
            JumpsLeft = 2;
        }

        if (Input.GetKeyDown(KeyCode.Space) && JumpsLeft > 0 && !isWallJumping)
        {
            if (JumpingDust.isPlaying) JumpingDust.Stop();
            if (!JumpingDust.isPlaying) JumpingDust.Play();
            JumpsLeft-=1;
            JumpAudio.Play();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));
            //canJump = false;
            isGrounded = false;
            if (JumpsLeft == 0)
            {
                anim.SetTrigger("DoubleJump");
            }
        }
        
        anim.SetBool("isGrounded", isGrounded);


    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight && !isWallJumping && !isWallJumping || horizontal < 0 && facingRight && !isWallSliding && !isWallJumping)
        {
            walljumpDirection *= -1;
            facingRight = !facingRight;
            transform.localScale= new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            RunningDustVelocity.x = -RunningDustVelocity.x.constantMax; //rotating Particle System
        }
    }

    private void WallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            JumpsLeft = 2;
        }
        anim.SetBool("isSliding", isTouchingWall);
    }

    private void WallJump()
    {
        if ((isWallSliding) && Input.GetKeyDown(KeyCode.Space))
        {
            JumpAudio.Play();
            JumpingDust.Play();
            rb.AddForce(new Vector2(walljumpforce * walljumpAngle.x * walljumpDirection, walljumpforce * walljumpAngle.y), ForceMode2D.Impulse);
            isWallJumping = true;
            walljumpDirection *= -1;
            facingRight = !facingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            RunningDustVelocity.x = -RunningDustVelocity.x.constantMax; //rotating Particle System
            JumpsLeft -= 1;

        }
    }

    private void CheckWorld()
    {
        //isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);

    }
    IEnumerator LoadLevelAfterDelay(float delay, string scene)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }

}