using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //---------Component--------//
    private Rigidbody2D _mRigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    //---------Player Attribute -----//
    private bool _mIsFacingRight = true;
    public float speed = 10f;
    public float jump = 16f;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private bool _isGrounded;
    private int _jumpRepeat = 1;
    private float _movementInputDirection;


    //--------Jump Attr----///
    public LayerMask whatIsGroundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;

    //-------Slider wall-----//
    public Transform wallcheck;
    private bool _isTouchingWall;
    public float wallCheckDistance;
    private bool _isSlidingWall;
    public float wallSlideSpeed;


    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckGround();
        FlipFace();
        WalkAnimation();
        JumpAnimation();
        Slidewall();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        _mRigidbody2D.velocity = new Vector2(_movementInputDirection * speed, _mRigidbody2D.velocity.y);
        if (_isSlidingWall)
        {
            
            if (_mRigidbody2D.velocity.y < -wallSlideSpeed)
            {
                _mRigidbody2D.velocity = new Vector2(_mRigidbody2D.velocity.x,wallSlideSpeed);
            }
        }
    }

    #region Jump

    void CheckInput()
    {
        _movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            JumpFocre();
            _jumpRepeat = 1;
        }
        else if (_jumpRepeat == 1)
        {
            JumpFocre();
            _jumpRepeat -= 1;
        }
    }

    private void JumpFocre()
    {
        _mRigidbody2D.velocity = new Vector2(_mRigidbody2D.velocity.x, jump);
    }

    private void JumpAnimation()
    {
        _animator.SetBool("isJump", _isGrounded);
        _animator.SetFloat("yVelocity", _mRigidbody2D.velocity.y);
    }

    #endregion

    #region Flip Player

    void FlipFace()
    {
        if (_mIsFacingRight && _movementInputDirection < 0)
        {
            _mIsFacingRight = false;
            FlipSprite(_mIsFacingRight);
        }
        else if (!_mIsFacingRight && _movementInputDirection > 0)
        {
            _mIsFacingRight = true;
            FlipSprite(_mIsFacingRight);
        }
    }

    private void FlipSprite(bool faceRight)
    {
        if (_isSlidingWall)
        {
            _spriteRenderer.flipX = !faceRight;
        }
       
    }

    #endregion

    #region Walk

    private void WalkAnimation()
    {
        if ((_movementInputDirection < 0 || _movementInputDirection > 0) && _isGrounded)
        {
            _animator.SetBool(IsWalking, true);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
            _animator.SetBool("IsGrounded",_isGrounded);
        }
    }

    #endregion

    #region Slide Wall

    private void Slidewall()
    {
        if (_isTouchingWall && !_isGrounded && _mRigidbody2D.velocity.y < 0)
        {
            _isSlidingWall = true;
        }
     
    }

    #endregion

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGroundLayer);
        _isTouchingWall = Physics2D.Raycast(wallcheck.position, transform.right, wallCheckDistance, whatIsGroundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallcheck.position,
            new Vector3(wallcheck.position.x + wallCheckDistance, wallcheck.position.y, wallcheck.position.z));
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }
}