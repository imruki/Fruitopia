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
    private int jumpsLeft;
    private float vSpeed;
    private bool isGrounded;

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
    [SerializeField] private AudioSource loseAudio;
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private AudioSource jumpAudio;
    [SerializeField] private AudioSource payAudio;

    [Header("Other")]
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead;
    [SerializeField] private CinemachineVirtualCamera myCamera;
    [SerializeField] private float knockbackForce;
    [SerializeField] private GameObject transition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private RuntimeAnimatorController[] characterAnimators;
    [SerializeField] private ParticleSystem runningDust;
    [SerializeField] private ParticleSystem.VelocityOverLifetimeModule runningDustVelocity;
    [SerializeField] private ParticleSystem jumpingDust;
    [SerializeField] private CharacterManager characterManager = null;


    void Awake()
    {
        // Make the game run as fast as possible in Windows
        Application.targetFrameRate = 300;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        runningDustVelocity = runningDust.velocityOverLifetime;
        isGrounded = true;
        jumpsLeft = 2;
        isDead = false;
        facingRight = true;
        isWallJumping = false;
        walljumpAngle.Normalize();

    }

    void Update()
    {
        if (isDead) return;
        CheckWorld();
        Inputs();
        WallJump();
        Jump();
    }
    private void FixedUpdate()
    {
        if (isDead) return;
        Movement();
        WallSlide();
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Ground"))
        {

            isGrounded = true;
            jumpsLeft = 2;
            isWallJumping = false;
        }                  
        if (other.gameObject.CompareTag("Enemy"))
        {
            loseAudio.Play();
            animator.SetTrigger("Hit");
            GetComponent<BoxCollider2D>().enabled = false;
            isDead = true;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, knockbackForce));
            myCamera.Follow = null;
            rb.constraints = RigidbodyConstraints2D.None;
            Instantiate(transition, centerPosition.position, Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(1.5f, SceneManager.GetActiveScene().name));
        }
        if (other.gameObject.CompareTag("finish"))
        {
            winAudio.Play();
            isGrounded = true;
            jumpsLeft = 2;
            isWallJumping = false;
        }


    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Level"))
        {
            Instantiate(transition,transform.position,Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(0.7f, other.gameObject.name));
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            winAudio.Play();
            Instantiate(transition, centerPosition.position, Quaternion.identity);
            StartCoroutine(LoadLevelAfterDelay(0.7f, "Lobby"));
        }
        
        else if (other.gameObject.CompareTag("Character"))
        {
            UnlockableCharacter chosenCharacter = new UnlockableCharacter();
            foreach (UnlockableCharacter character in characterManager.GetCharacters())
            {
                if (character.getName() == other.gameObject.name)
                {
                    chosenCharacter = character;
                    break;
                }
            }

            if (gameObject.name != chosenCharacter.getName() && CoinManager.coins >= chosenCharacter.getPrice() && animator.runtimeAnimatorController != chosenCharacter.GetAnimator()) {
                payAudio.Play();
                CoinManager.coins -= chosenCharacter.getPrice();
                animator.runtimeAnimatorController = chosenCharacter.GetAnimator().runtimeAnimatorController;
                gameObject.name = chosenCharacter.getName();
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
        animator.SetFloat("vSpeed", vSpeed);
        Flip(XDirectionalInput);
        if (isGrounded)
        {
            rb.velocity = new Vector2(XDirectionalInput * playerSpeed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(XDirectionalInput));
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
            if (!runningDust.isPlaying) runningDust.Play();
        }
        else
        {
            if (runningDust.isPlaying) runningDust.Stop();


        }
    }



    private void Jump()
    {
        if (isGrounded) {
            jumpsLeft = 2;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0 && !isWallJumping)
        {
            if (jumpingDust.isPlaying) jumpingDust.Stop();
            if (!jumpingDust.isPlaying) jumpingDust.Play();
            jumpsLeft-=1;
            //jumpAudio.Play();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));
            isGrounded = false;
            if (jumpsLeft == 0)
            {
                animator.SetTrigger("DoubleJump");
            }
        }
        
        animator.SetBool("isGrounded", isGrounded);


    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight && !isWallJumping && !isWallJumping || horizontal < 0 && facingRight && !isWallSliding && !isWallJumping)
        {
            walljumpDirection *= -1;
            facingRight = !facingRight;
            transform.localScale= new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            runningDustVelocity.x = -runningDustVelocity.x.constantMax; //rotating Particle System
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
            jumpsLeft = 2;
        }
        animator.SetBool("isSliding", isTouchingWall);
    }

    private void WallJump()
    {
        if ((isWallSliding) && Input.GetKeyDown(KeyCode.Space))
        {
            //jumpAudio.Play();
            jumpingDust.Play();
            rb.AddForce(new Vector2(walljumpforce * walljumpAngle.x * walljumpDirection, walljumpforce * walljumpAngle.y), ForceMode2D.Impulse);
            isWallJumping = true;
            walljumpDirection *= -1;
            facingRight = !facingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            runningDustVelocity.x = -runningDustVelocity.x.constantMax; //rotating Particle System
            jumpsLeft -= 1;

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