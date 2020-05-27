using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    public static int maxSpawn = 750;
    public static float maxSpeed = 50;

    [SerializeField]
    private GameObject fish;
    [SerializeField, Range(100, 750)]
    private int initSpawnCount = 250;
    [SerializeField, Range(20, 50)]
    private float initFishSpeed = 20;
    [SerializeField, Range(0, 1)]
    private float avoidStrenth = 0.1f;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private Gradient fishGradient;

    public static bool followTarget = true;
    public static Vector2 target = Vector2.zero;
    private Camera cam;

    private Queue<GameObject> activeQueue = new Queue<GameObject>();
    private Queue<GameObject> sleepQueue = new Queue<GameObject>();

    private void Awake()
    {
        Fish.maxX = transform.localScale.x / 2f;
        Fish.minX = -transform.localScale.x / 2f;
        Fish.maxY = transform.localScale.y / 2f;
        Fish.minY = -transform.localScale.y / 2f;
        Fish.speed = initFishSpeed;
        Fish.strenth = avoidStrenth;
        Fish.obstacleLayer = obstacleLayer;
        Fish.gradient = fishGradient;
    }


    void Start()
    {
        cam = Camera.main;
        float maxX = transform.localScale.x / 2f;
        float minX = -transform.localScale.x / 2f;
        float maxY = transform.localScale.y / 2f;
        float minY = -transform.localScale.y / 2f;

        for (int i = 0; i < maxSpawn; i++)
        {
            GameObject go = Instantiate(fish, new Vector3(Random.Range(minX + 1, maxX - 1), Random.Range(minY + 1, maxY - 1)), Quaternion.identity);
            if (i < initSpawnCount)
                activeQueue.Enqueue(go);
            else
            {
                go.SetActive(false);
                sleepQueue.Enqueue(go);
            }
        }
    }

    private void FixedUpdate()
    {
        if (followTarget)
            target = cam.ScreenToWorldPoint(Input.mousePosition);
        else
            target = new Vector2(10000, 10000);
        
        if(activeQueue.Count > Fish.spawnCount)
        {
            GameObject go = activeQueue.Dequeue();
            go.SetActive(false);
            sleepQueue.Enqueue(go);
        }
        else if(activeQueue.Count < Fish.spawnCount)
        {
            GameObject go = sleepQueue.Dequeue();
            go.SetActive(true);
            activeQueue.Enqueue(go);
        }
    }
}


