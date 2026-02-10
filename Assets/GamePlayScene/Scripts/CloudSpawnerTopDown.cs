using UnityEngine;

public class CloudSpawnerTopDown : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] cloudPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnIntervalMin = 1.2f;
    [SerializeField] private float spawnIntervalMax = 2.5f;
    [SerializeField] private float spawnOffsetAbove = 2f;   // насколько выше экрана спавнить

    [Header("Movement")]
    [SerializeField] private float speedMin = 0.6f;
    [SerializeField] private float speedMax = 1.8f;

    [Header("Scale")]
    [SerializeField] private float scaleMin = 0.7f;
    [SerializeField] private float scaleMax = 1.4f;

    [Header("Destroy Settings")]
    [SerializeField] private float destroyOffsetBelow = 3f; // насколько ниже экрана удалять

    private float timer;

    private void Start()
    {
        if (!cam) cam = Camera.main;
        ScheduleNext();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnCloud();
            ScheduleNext();
        }
    }

    private void ScheduleNext()
    {
        timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    private void SpawnCloud()
    {
        if (cloudPrefabs.Length == 0) return;

        var prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));

        bool fromLeft = Random.value > 0.5f;

        float y = Random.Range(left.y, top.y);

        float x;
        int dir;

        if (fromLeft)
        {
            x = left.x - 2f;
            dir = 1; // вправо
        }
        else
        {
            x = right.x + 2f;
            dir = -1; // влево
        }

        GameObject cloud = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);

        float speed = Random.Range(0.6f, 1.8f);

        var mover = cloud.GetComponent<CloudMover>();
        if (mover == null) mover = cloud.AddComponent<CloudMover>();

        mover.Init(dir, 3f);
    }

}
