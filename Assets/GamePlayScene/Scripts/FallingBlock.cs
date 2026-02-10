using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FallingBlock : MonoBehaviour
{
    public event Action<FallingBlock> Landed;
    public event Action<FallingBlock> Missed;

    [Header("Landing / Miss")]
    [SerializeField] private float settleVelocityThreshold = 0.12f;
    [SerializeField] private float settleTime = 0.25f;

    [Header("Miss settings")]
    [SerializeField] private float missCheckDelayAfterRelease = 0.15f;

    [Header("SFX")]
    [Tooltip("Играть звук при первом касании другого блока (PlacedBlock).")]
    [SerializeField] private bool playTouchSound = true;

    private Rigidbody2D rb;

    private bool released;
    private bool landed;
    private float settleTimer;

    private float missY;
    private float missDelayTimer;

    private bool touchSoundPlayed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ArmMissLine(float worldY)
    {
        missY = worldY;
    }

    public void AttachToHook(Transform hookPoint)
    {
        released = false;
        landed = false;
        settleTimer = 0f;
        missDelayTimer = 0f;
        touchSoundPlayed = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        transform.position = hookPoint.position;
        transform.rotation = hookPoint.rotation;
        transform.SetParent(hookPoint, true);
    }

    public void Release()
    {
        if (released) return;

        released = true;
        missDelayTimer = missCheckDelayAfterRelease;

        transform.SetParent(null, true);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        if (!released || landed) return;

        if (missDelayTimer > 0f)
        {
            missDelayTimer -= Time.deltaTime;
            return;
        }

        if (transform.position.y < missY)
        {
            Missed?.Invoke(this);
        }
    }

    // 🔊 ЗВУК: при первом касании другого блока
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!playTouchSound) return;
        if (!released) return;              // пока висит на HookPoint — не надо
        if (touchSoundPlayed) return;       // один раз

        // играем только если коснулись другого блока (уже поставленного)
        if (collision.collider != null && collision.collider.CompareTag("PlacedBlock"))
        {
            touchSoundPlayed = true;
            SoundManager.Instance?.PlayBlockPlace();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!released || landed) return;

        if (rb.linearVelocity.magnitude < settleVelocityThreshold && Mathf.Abs(rb.angularVelocity) < 15f)
        {
            settleTimer += Time.deltaTime;
            if (settleTimer >= settleTime)
            {
                landed = true;

                // ВАЖНО: после установки помечаем блок как "PlacedBlock"
                gameObject.tag = "PlacedBlock";

                Landed?.Invoke(this);
            }
        }
        else
        {
            settleTimer = 0f;
        }
    }
}
