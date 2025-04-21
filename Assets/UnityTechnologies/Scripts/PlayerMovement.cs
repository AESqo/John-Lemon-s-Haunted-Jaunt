using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    
    public float turnSpeed = 20f;
    private bool run = false;
    private bool tired = false;
    private float stamina = 100f;
    public TextMeshProUGUI staminaText;
    bool isWalking = false;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        
        MoveAction.Enable();
    }

    void FixedUpdate ()
    {
        if (stamina == 100)
        {
            staminaText.text = "";
        }
        else
        {
            staminaText.text = "Stamina: " + stamina;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) && !tired)
        {
            run = true;
        }
        else
        {
            run = false;
        }
        var pos = MoveAction.ReadValue<Vector2>();
        
        float horizontal = pos.x;
        float vertical = pos.y;
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);
        
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);
    }

    void OnAnimatorMove ()
    {
        if (run && !tired && isWalking)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * (m_Animator.deltaPosition.magnitude * 2));
            m_Rigidbody.MoveRotation(m_Rotation);
            stamina -= 1f;
            if (stamina < 1f)
            {
                tired = true;
            }
        }
        else if (tired)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * (m_Animator.deltaPosition.magnitude / 2));
            m_Rigidbody.MoveRotation(m_Rotation);
            if (stamina < 100)
            {
                stamina += 0.5f;
            }
            if (tired && stamina == 100)
            {
                tired = false;
            }
        }
        else
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
            m_Rigidbody.MoveRotation(m_Rotation);
            if (stamina < 100)
            {
                stamina += 1f;
            }
        }
    }
}