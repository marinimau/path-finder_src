﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
    
    public float WaitTime = 0.5f;
    float standard_walkspeed = 2;
    float reloading_walkspeed = 0.25f;
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    public float timerJump = 0;
    public float timeRientroRinculo=2f;
    public float smooth;
    Quaternion old_rotation;
    [Range(0, 1)]
    public float airControlPercent;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    

    private bool load=false;
    private bool load_data = false;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    public int mouseSensitivity = 10;

    float airTime;
    bool isJumping;
    float staticJumpBuff;
    private float bigJumpTime;

    public static int health;
    public static bool isDead;
    public static bool immortality;
    public static float immortalityTimer;
    public static bool invisible;
    public static float invisibleTimer;
    public static bool specialBullet;
    public static float specialBulletTimer;

    public bool isReloading;

    public Animator animator;
    Transform cameraT;
    CharacterController controller;

    public static bool player_contact;
    public static bool boss_contact;
    public static bool player_contact_deactivated;

    //danni da caduta
    public bool bigJump;
    public float jumpTimeStart; 

    public Vector2 pitchMinMax = new Vector2(-40, 85);
    float lastRotation; //Serve a resettare la posizione del personaggio durante la mira sull'asse verticale
    public Boolean flag = false;

    public static ParticleSystem PlayerBlood;

    float targetSpeed;
    public static Boolean fire = false;
    public Renderer mesh;
    public Material materialMesh;
    public Material invisibleMaterial;

    public static bool gameOver = false;

    public static bool key;
    public static bool reset;


    // Start is called before the first frame update
    void Start()
    {
        old_rotation = transform.rotation;
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        PlayerBlood = GetComponentInChildren<ParticleSystem>();
        health = 100;
        isDead = false;
        immortality = false;
        immortalityTimer = 0;
        invisible = false;
        invisibleTimer = 0;
        specialBullet = false;
        specialBulletTimer = 0;
        bigJump = false;
        //mesh= gameObject.transform.GetChild(5).GetComponent<Renderer>();
        mesh = gameObject.transform.GetChild(5).GetComponent<Renderer>();
        materialMesh = mesh.material;
        player_contact = false;
        boss_contact = false;
        player_contact_deactivated = false;
        key = false;
        isReloading = false;
        reset = false;


    }

    // Update is called once per frame
    void Update()
    {
        if(reset){
            cameraT = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            reset = false;
        }

        if (!PauseMenu.isPaused)
        {
            if (invisible)
            {
                if (mesh.material == materialMesh)
                {
                    mesh.material = invisibleMaterial;
                }
                Debug.Log("Player invisibile");
                invisibleTimer -= Time.deltaTime * 0.1f;
                if (invisibleTimer <= 0)
                {
                    ShowMessage.id = 5;
                    Debug.Log("Player visibile");
                    invisible = false;

                }
            }
            else
            {
                if (mesh.material != materialMesh)
                {
                    mesh.material = materialMesh;
                }
            }

            if (specialBullet)
            {
                specialBulletTimer -= Time.deltaTime * 0.1f;
                if (specialBulletTimer <= 0)
                {
                    ShowMessage.id = 12;
                    Debug.Log("Potenza pistola normale");
                    specialBullet = false;
                }
            }

            if (immortality)
            {
                immortalityTimer -= Time.deltaTime * 0.1f;
                if (immortalityTimer <= 0)
                {
                    ShowMessage.id = 6;
                    Debug.Log("Player MORTALE");
                    immortality = false;
                }
            }


            bool running = Input.GetKey(KeyCode.LeftShift);

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            if (!isDead)
            {
                if (Input.GetButton("Fire2"))
                {
                    if (Input.GetAxis("Jump") > 0 && currentSpeed <= 0.1f && !isJumping)
                    {
                        animator.SetBool("jumpStatic", true);
                        StartCoroutine("Jump_Static_Land", WaitTime);

                    }
                    else
                    {
                        if (Input.GetAxis("Jump") > 0)
                        {
                            JumpWhileAiming();
                            animator.SetBool("Jumping", true);

                        }
                    }
                    MoveWhileAiming(inputDir, running);
                }
                else
                {

                    if (flag)
                    {
                        transform.localEulerAngles = new Vector3(0, lastRotation, 0);
                        flag = false;
                    }

                    if (Input.GetAxis("Jump") > 0 && currentSpeed <= 0.1f)
                    {
                        animator.SetBool("jumpStatic", true);
                        StartCoroutine("Jump_Static_Land", WaitTime);

                    }
                    else
                    {
                        if (Input.GetAxis("Jump") > 0)
                        {
                            Jump();
                            animator.SetBool("Jumping", true);

                        }
                    }
                    //input per movimento
                    Move(inputDir, running);
                }


                if (Input.GetButtonDown("Fire1") && !fire && !GunScript.armaScarica)
                {
                    //AnimazioneSparo();
                    Recoil.recoilActive = true;
                }

                if (fire)
                {
                    smooth += Time.deltaTime * 4F;
                    transform.rotation = Quaternion.Lerp(transform.rotation, old_rotation, smooth);
                    if (smooth > 1)
                    {
                        fire = false;
                    }
                }


                //animator
                float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
                animator.SetFloat("speedPercentage", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

            }
            else
            {
                //Il player è morto
                if (gameOver == false)
                {
                    Debug.Log("Sono morto una volta");
                    health = 0;
                    gameOver = true;
                    animator.SetBool("dead", true);
                    player_contact_deactivated = true;
                    player_contact = false;
                    boss_contact = false;
                }

            }
        }
        

    }

    void AnimazioneSparo(){
        if(!fire){
            smooth = 0;
            old_rotation = transform.rotation;
            transform.Rotate(new Vector3(-5, 0, 0));
            fire = true;
        }
    }



    void Move(Vector2 inputDir, bool running)
    {
        gravity = -12;
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        targetSpeed = ((running && !isReloading) ? runSpeed : walkSpeed) * inputDir.magnitude;     //Se stiamo correndo allora la velocità sarà uguale a runspeed, altrimenti a walkspeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));       //solo asse x e z

        velocityY += Time.deltaTime * gravity;      //velocità asse y calcolata a parte.

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            animator.SetBool("Jumping", false);

            velocityY = 0;
            airTime = 0;
            animator.SetBool("airTime", false);
            isJumping = false;
        }

        if (!controller.isGrounded)
        {
            airTime += Time.deltaTime;


            if((airTime > 0.6f && isJumping == true) || (airTime > 0.3f && isJumping == false))
            {
                animator.SetBool("airTime", true);
                if (!bigJump)
                {
                    Debug.Log("caduta");
                    bigJump = true;
                    timerJump = Time.time;
                }
            }



        }

        if(controller.isGrounded && bigJump){
            bigJump = false;
            if(Time.time-timerJump>=0){
                Debug.Log("tempo di salto " + (Time.time - timerJump));
                bigJumpTime = (float)((Time.time - timerJump));
                if(bigJumpTime>0.6){
                    int quantityOfDamage = (int)(bigJumpTime * 40.0);
                    decrHealth(quantityOfDamage);
                    Talk.id = 1;
                }

            }
            else{
                Talk.id = 6;
            }

        }

    }


    void MoveWhileAiming(Vector2 inputDir, bool running)
    {

        animator.SetBool("airTime", false);

        gravity = controller.isGrounded && Input.GetAxis("Horizontal") <= 0.5f && Input.GetAxis("Vertical") <= 0.5f ? -1f : -12f;

        Vector2 inputMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        //float max = 0, min = 0;
        if (inputMouse.x != 0)
        {
            //rotazione sull'asse y
            transform.Rotate(new Vector3(0, inputMouse.x * mouseSensitivity, 0), Space.World);
            lastRotation = transform.eulerAngles.y;
            flag = true;
        }

        if (inputMouse.y != 0 && ((transform.eulerAngles.x - inputMouse.y * mouseSensitivity) >= float.MinValue
            && transform.eulerAngles.x - inputMouse.y * mouseSensitivity < 45)
            || (transform.eulerAngles.x - inputMouse.y * mouseSensitivity >= 320
            && transform.eulerAngles.x - inputMouse.y * mouseSensitivity <= float.MaxValue))
        {
            //rotazioni sull'asse x
            transform.Rotate(new Vector3(-inputMouse.y * mouseSensitivity, 0, 0));

        }

        Vector3 moveDirection;
        targetSpeed = ((running && !isReloading) ? runSpeed : walkSpeed) * inputDir.magnitude;     //Se stiamo correndo allora la velocità sarà uguale a runspeed, altrimenti a walkspeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));       //solo asse x e z
        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        moveDirection = new Vector3(inputDir.x, velocityY, inputDir.y);

        // Debug.Log(" ");
        moveDirection = transform.TransformDirection(moveDirection);

        if (running == false)
        {
            moveDirection = moveDirection * walkSpeed;
        }
        else
        {
            moveDirection = moveDirection * runSpeed;
        }

        controller.Move(moveDirection * Time.deltaTime);

        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            animator.SetBool("Jumping", false);

            velocityY = 0;
            airTime = 0;
            //animator.SetBool("airTime", false);
            isJumping = false;
        }

        if (!controller.isGrounded)
        {
            airTime += Time.deltaTime;


            if ((airTime > 1.2f && isJumping == true) || (airTime > 0.3f && isJumping == false))
            {
                animator.SetBool("airTime", true);
                if (!bigJump)
                {
                    Debug.Log("caduta");
                    bigJump = true;
                    timerJump = Time.time;
                }
            }



        }

        if (controller.isGrounded && bigJump)
        {
            bigJump = false;
            if (Time.time - timerJump >= 0)
            {
                Debug.Log("tempo di salto " + (Time.time - timerJump));
                bigJumpTime = (float)((Time.time - timerJump));
                if (bigJumpTime > 0.4 || Input.GetKey(KeyCode.LeftShift))
                {
                    int quantityOfDamage;
                    if (Input.GetKey(KeyCode.LeftShift)){
                        quantityOfDamage = (int)(bigJumpTime * 150.0);
                    } else {
                        quantityOfDamage = (int)(bigJumpTime * 50.0);
                    }

                    decrHealth(quantityOfDamage);
                    Talk.id = 1;
                }

            }
        }



    }


    void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            isJumping = true;
        }
    }


    void JumpWhileAiming()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity/2;
            isJumping = true;
            timerJump = 0.3f;
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }

        return smoothTime / airControlPercent;
    }

    IEnumerator Jump_Static_Land(float Count)
    {
        yield return new WaitForSeconds(Count);
        JumpWhileAiming();
        yield return new WaitForSeconds(Count);
        animator.SetBool("jumpStatic", false);

        yield return null;
    }

    public void setReloadingWalkSpeed()
    {
        this.walkSpeed = reloading_walkspeed;
    }

    public void setStandardWalkSpeed()
    {
        this.walkSpeed = standard_walkspeed;
        animator.SetFloat("speedPercentage", 0f);
    }

    public void standStill()
    {
        this.walkSpeed = 0.0f;
    }

    public static void decrHealth(int damage){
        if(health-damage>0){
            health -= damage;
        } else{
            isDead = true;
        }

    }

    public static void incHealth(int cura)
    {
        if (health + cura <=100)
        {
            health+= cura;
        }
        else
        {
            health = 100;
        }
    }

    private void OnOnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("DeathZone"))
            decrHealth(100);
            //Debug.Log("MUORI");
        }
}
