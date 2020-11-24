using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public class blink : MonoBehaviour
{
    [SerializeField] AudioClip blinkClip;
    [SerializeField] Camera cameraController;
    [SerializeField] Transform rayOrigin;
    [SerializeField] float shootDistance = 10f;
    [SerializeField] float blinkDistance = 30f;
    [SerializeField] GameObject player;
    [SerializeField] float blinkSpeed = 10f;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Transform groundCheck;
    public GameObject blinkEffect;
    public PostProcessLayer ppLayer;
    [SerializeField] GameObject hand;
    [SerializeField] GameObject fist;
    [SerializeField] Image _manaBar;
    public float regenMana;
    public float manaPerSecond = 100;
    AudioSource _blinkAudio;
    int mana = 500;

    RaycastHit blinkHit;
    RaycastHit effectHit;
    Vector3 target;
    bool isGrounded;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    // Update is called once per frame
    private void Awake()
    {
        _blinkAudio = GetComponent<AudioSource>();
        _manaBar.rectTransform.sizeDelta = new Vector2(100, mana);
    }
    private void Update()
    {
        if (isGrounded && mana < 500)
        {
            regenMana += Time.deltaTime * manaPerSecond;
        }
        if (regenMana >= 1 && mana < 500)
        {
            int floor = Mathf.FloorToInt(regenMana);
            mana += floor; // normally 1 unless huge framedrop or Huge regenHealth value
            regenMana -= floor;
        }
        _manaBar.rectTransform.sizeDelta = new Vector2(100, mana);
        Vector3 rayDirection = cameraController.transform.forward;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Physics.Raycast(rayOrigin.position, rayDirection, out effectHit, blinkDistance);
        Ray effectPoint = new Ray(rayOrigin.position, rayDirection);
        target = effectPoint.GetPoint(blinkDistance);
        if (Physics.Raycast(rayOrigin.position, rayDirection, blinkDistance))
        {
            blinkEffect.transform.position = effectHit.point;
        }
        else
        {
            blinkEffect.transform.position = target;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            BlinkPoint();
        }
        if (Input.GetKey(KeyCode.Mouse1) && !isGrounded)
        {
            Debug.Log("TIMESTOP");
            ppLayer.enabled = true;
            Time.timeScale = 0;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            blinkEffect.SetActive(true);
            hand.SetActive(false);
            fist.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            blinkEffect.SetActive(false);
            if (mana >= 0)
            {
                Blink();
            }
            else {
                Time.timeScale = 1;
                ppLayer.enabled = false;
                    }
            fist.SetActive(false);
            hand.SetActive(true);
            
        }
    }

    void BlinkPoint()
    {
        Vector3 rayDirection = cameraController.transform.forward;
        Debug.DrawRay(rayOrigin.position, rayDirection * shootDistance, Color.red, 1f);
        if (Physics.Raycast(rayOrigin.position, rayDirection, out blinkHit, blinkDistance))
        {
        }
        else
        {
            Ray blinkPoint = new Ray(rayOrigin.position, rayDirection);
            target = blinkPoint.GetPoint(blinkDistance);
        }
        


    }
    void Blink()
    {
        Vector3 rayDirection = cameraController.transform.forward;
        Debug.DrawRay(rayOrigin.position, rayDirection * shootDistance, Color.red, 1f);
        if (Physics.Raycast(rayOrigin.position, rayDirection, out blinkHit, blinkDistance))
        {
            if (Physics.Raycast(rayOrigin.position, rayDirection, blinkDistance))
            {
                var moveVector = blinkHit.point - player.transform.position;
                playerMovement.velReset();
                Time.timeScale = 1;
                    moveVector = moveVector.normalized * blinkSpeed;
                    player.GetComponent<CharacterController>().Move(moveVector);
                playerMovement.velReset();
                ppLayer.enabled = false;
                Mana();
                playBlink(blinkClip);
            }
        }
        else
        {
            Ray blinkPoint = new Ray(rayOrigin.position, rayDirection);
            target = blinkPoint.GetPoint(blinkDistance);
            playerMovement.velReset();
            var moveVector = target - player.transform.position;
            Time.timeScale = 1;
                player.GetComponent<CharacterController>().Move(moveVector);
            playerMovement.velReset();
            ppLayer.enabled = false;
            Mana();
            playBlink(blinkClip);

        }

    }
    public void Mana()
    {
        mana -= 100;
        
    }
    public void playBlink(AudioClip blinkClip)
    {
        _blinkAudio.clip = blinkClip;
        _blinkAudio.Play();
    }
}
