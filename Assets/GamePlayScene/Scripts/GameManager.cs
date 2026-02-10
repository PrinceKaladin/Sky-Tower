using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Links")]
    [SerializeField] private CanvasGameController canvas;
    [SerializeField] private RopeController rope;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Ground Reference")]
    [SerializeField] private Transform groundTransform;

    [Header("Spawn")]
    [SerializeField] private FallingBlock[] blockPrefabs;
    [SerializeField] private float spawnDelay = 0.15f;

    [Header("Lose Conditions")]
    [SerializeField] private float missBelowGround = 6f;
    [SerializeField] private float collapseBelowGround = 2.0f;
    [SerializeField] private float collapseGraceTime = 0.8f;
    [SerializeField] private int minBlocksBeforeCollapseCheck = 2;

    private int score = 0;
    private bool gameOver = false;

    private readonly List<FallingBlock> placedBlocks = new List<FallingBlock>();
    private FallingBlock currentBlock;

    private float highestPlacedY = 0f;
    private float collapseIgnoreUntilTime = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (!canvas) canvas = FindFirstObjectByType<CanvasGameController>();
        if (!rope) rope = FindFirstObjectByType<RopeController>();
        if (!cameraFollow) cameraFollow = FindFirstObjectByType<CameraFollow>();

        StartNewGame();
    }

    public void StartNewGame()
    {
        gameOver = false;
        score = 0;
        highestPlacedY = 0f;
        collapseIgnoreUntilTime = 0f;

        placedBlocks.Clear();

        Time.timeScale = 1f;
        if (canvas) canvas.HideAllMenu();
        if (canvas) canvas.UpdateScore(score);

        if (rope) rope.ResetToStartInstant();
        SpawnNextBlock();
    }

    private void Update()
    {
        if (gameOver) return;

        if (placedBlocks.Count < minBlocksBeforeCollapseCheck) return;
        if (Time.time < collapseIgnoreUntilTime) return;

        float groundY = groundTransform ? groundTransform.position.y : 0f;
        float collapseLine = groundY - collapseBelowGround;

        for (int i = placedBlocks.Count - 1; i >= 0; i--)
        {
            var b = placedBlocks[i];
            if (!b) { placedBlocks.RemoveAt(i); continue; }

            if (b.transform.position.y < collapseLine)
            {
                Lose("Tower collapsed");
                return;
            }
        }
    }

    private void SpawnNextBlock()
    {
        if (gameOver) return;

        if (blockPrefabs == null || blockPrefabs.Length == 0)
        {
            Debug.LogError("GameManager: blockPrefabs is empty!");
            return;
        }

        var prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        currentBlock = Instantiate(prefab);

        if (!rope || rope.HookPoint == null)
        {
            Debug.LogError("GameManager: Rope or HookPoint not set!");
            return;
        }

        currentBlock.AttachToHook(rope.HookPoint);

        float groundY = groundTransform ? groundTransform.position.y : 0f;
        currentBlock.ArmMissLine(groundY - missBelowGround);

        currentBlock.Landed += OnBlockLanded;
        currentBlock.Missed += OnBlockMissed;

        rope.SetCurrentBlock(currentBlock);
    }

    private void OnBlockMissed(FallingBlock b)
    {
        if (gameOver) return;
        Lose("Block missed");
    }

    private void OnBlockLanded(FallingBlock b)
    {
        if (gameOver) return;

        b.Landed -= OnBlockLanded;
        b.Missed -= OnBlockMissed;

        placedBlocks.Add(b);

        score += 1;
        if (canvas) canvas.UpdateScore(score);

        if (b.transform.position.y > highestPlacedY)
            highestPlacedY = b.transform.position.y;

        if (cameraFollow)
            cameraFollow.NotifyNewPlacedHeight(b.transform.position.y);

        collapseIgnoreUntilTime = Time.time + collapseGraceTime;

        if (rope) rope.ResetToStartInstant();

        Invoke(nameof(SpawnNextBlock), spawnDelay);
    }

    private void Lose(string reason)
    {
        if (gameOver) return;
        gameOver = true;

        if (rope) rope.ResetToStartInstant();
        if (canvas) canvas.ShowGameOverMenu(score);

        Debug.Log($"GAME OVER: {reason}. Score={score}");
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        StartNewGame();
    }
}
