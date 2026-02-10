using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float smoothUp = 6f;
    [SerializeField] private float smoothDown = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Up step logic")]
    [SerializeField] private float stepUpPerBlock = 1.0f;
    [SerializeField] private float maxUpJumpPerPlace = 1.5f;

    [Header("Ignore first blocks")]
    [SerializeField] private int ignoreFirstBlocks = 3; // ? первые 3 блока без подъёма

    [Header("Down follow limit")]
    [SerializeField] private float maxDownFromPeak = 2.5f;

    private float peakY;
    private float desiredY;

    private float lastPlacedY = float.NegativeInfinity;
    private int placedCount = 0;

    private void Start()
    {
        peakY = transform.position.y;
        desiredY = transform.position.y;
    }

    /// <summary>
    /// Вызывать при установке блока
    /// </summary>
    public void NotifyNewPlacedHeight(float placedBlockWorldY)
    {
        placedCount++;

        // ? игнорируем первые N блоков
        if (placedCount <= ignoreFirstBlocks)
        {
            lastPlacedY = placedBlockWorldY;
            return;
        }

        if (float.IsNegativeInfinity(lastPlacedY))
        {
            lastPlacedY = placedBlockWorldY;
            return;
        }

        float delta = placedBlockWorldY - lastPlacedY;
        lastPlacedY = placedBlockWorldY;

        float step = Mathf.Max(stepUpPerBlock, delta);
        step = Mathf.Clamp(step, 0f, maxUpJumpPerPlace);

        desiredY += step;
    }

    public float GetBottomWorldY()
    {
        var cam = GetComponent<Camera>();
        if (!cam) return transform.position.y - 10f;
        return cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).y;
    }

    private void LateUpdate()
    {
        float currentY = transform.position.y;
        float targetY = desiredY;

        float newY;

        if (targetY > currentY)
        {
            newY = Mathf.Lerp(currentY, targetY, 1f - Mathf.Exp(-smoothUp * Time.deltaTime));
        }
        else
        {
            float minAllowed = peakY - maxDownFromPeak;
            float clamped = Mathf.Max(targetY, minAllowed);
            newY = Mathf.Lerp(currentY, clamped, 1f - Mathf.Exp(-smoothDown * Time.deltaTime));
        }

        if (newY > peakY) peakY = newY;

        transform.position = new Vector3(offset.x, newY, offset.z);
    }
}
