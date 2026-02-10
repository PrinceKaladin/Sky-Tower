using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speedMin = 0.1f;
    [SerializeField] private float speedMax = 0.3f;

    [SerializeField] private float destroyOffsetSide = 3f;

    private float speed;
    private Camera cam;
    private int direction = 1;

    public void Init(int moveDir, float offsetSide)
    {
        direction = moveDir;
        destroyOffsetSide = offsetSide;

        // ✅ скорость генерим тут
        speed = Random.Range(speedMin, speedMax);
    }

    private void Awake()
    {
        FindCameraByTag();
    }

    private void FindCameraByTag()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cam = camObj.GetComponent<Camera>();
    }

    private void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (cam == null)
        {
            FindCameraByTag();
            return;
        }

        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));

        if (direction == 1 && transform.position.x > right.x + destroyOffsetSide)
            Destroy(gameObject);

        if (direction == -1 && transform.position.x < left.x - destroyOffsetSide)
            Destroy(gameObject);
    }
}
