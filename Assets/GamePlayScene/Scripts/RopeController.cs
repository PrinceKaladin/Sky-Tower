using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeController : MonoBehaviour
{
    public enum RopeState { Moving, Dropping }

    [Header("HookPoint (child of Cable)")]
    [SerializeField] private Transform hookPoint; // HookPoint (дочерний объект Cable)

    [Header("Cable local position relative to Hook")]
    [SerializeField] private float localYUnderHook = -2.63f; // <-- твое значение

    [Header("Cable Movement (local Z)")]
    [SerializeField] private float minLocalZ = -40f;
    [SerializeField] private float maxLocalZ = 40f;
    [SerializeField] private float moveSpeed = 15f;

    [Header("Swing Rotation (local Z angle)")]
    [SerializeField] private float swingAngle = 40f; // +-40°
    [SerializeField] private float swingSpeed = 2f;

    [Header("Drop Physics")]
    [SerializeField] private float dropGravityScale = 2f;
    [SerializeField] private KeyCode dropKey = KeyCode.Space;

    private Rigidbody2D rb;
    private RopeState state = RopeState.Moving;

    private int dir = 1;
    private float swingTimer;

    private FallingBlock currentBlock;

    // запоминаем стартовые локальные X/Z (Y задаём фиксировано -2.63)
    private float startLocalX;
    private float startLocalZ;

    public Transform HookPoint => hookPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // запоминаем стартовые X/Z, чтобы старт был "как в сцене"
        startLocalX = transform.localPosition.x;
        startLocalZ = transform.localPosition.z;

        // при запуске выставляем нужный local Y
        var lp = transform.localPosition;
        transform.localPosition = new Vector3(lp.x, localYUnderHook, lp.z);
    }

    private void Update()
    {
        if (state != RopeState.Moving) return;

        MoveLocalZ();
        SwingLocalRotation();

        if (Input.GetKeyDown(dropKey))
            Drop();
    }

    private void MoveLocalZ()
    {
        float z = transform.localPosition.z + dir * moveSpeed * Time.deltaTime;

        if (z > maxLocalZ) { z = maxLocalZ; dir = -1; }
        if (z < minLocalZ) { z = minLocalZ; dir = 1; }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    private void SwingLocalRotation()
    {
        swingTimer += Time.deltaTime * swingSpeed;
        float angle = Mathf.Sin(swingTimer) * swingAngle;

        // вращаем локально вокруг Z
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetCurrentBlock(FallingBlock block)
    {
        currentBlock = block;
    }

    public void Drop()
    {
        if (state != RopeState.Moving) return;

        state = RopeState.Dropping;

        if (currentBlock != null)
            currentBlock.Release();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = dropGravityScale;
    }

    /// <summary>
    /// Сброс Cable в исходное положение относительно Hook:
    /// localY = -2.63, localZ = стартовое (или можно поставить 0), сброс поворота.
    /// </summary>
    public void ResetToStartInstant()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // возвращаемся под Hook (локально)
        transform.localPosition = new Vector3(startLocalX, localYUnderHook, startLocalZ);

        // сброс качания и поворота
        swingTimer = 0f;
        transform.localRotation = Quaternion.identity;

        // заново начинаем движение
        state = RopeState.Moving;
        dir = 1;
    }
}
