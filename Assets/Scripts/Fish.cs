using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities; 

public class Fish : MonoBehaviour
{
    public static float radius = 5;
    public static float minX, maxX, minY, maxY;
    public static float speed;
    public static float spawnCount;
    public static float strenth;
    public static float viewAngle = 135;
    public static LayerMask obstacleLayer;
    public static Gradient gradient;

    public static Dictionary<Collider2D, Fish> fishDict = new Dictionary<Collider2D, Fish>();

    private SpriteRenderer sr;
    public Vector2 direction = Vector2.zero;
    private int updateMark;
    private int counter;

    private void Awake()
    {
        fishDict.Add(GetComponent<Collider2D>(), this);
    }

    void Start()
    {
        float seed = Random.Range(0, 2 * Mathf.PI);
        direction = new Vector2(Mathf.Cos(seed), Mathf.Sin(seed));
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = gradient.Evaluate(seed / (2 * Mathf.PI));
        updateMark = Random.Range(0,4);
    }

    public void FixedUpdate()
    {
        if (FishSpawner.target.magnitude < 10000)
        {
            Vector2 followDir = (FishSpawner.target - (Vector2)transform.position).normalized;
            direction = Vector2.Lerp(direction, followDir, strenth/5f).normalized;
        }
        if (counter % 4 == updateMark)
        { 
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            Vector2 clusterVector = Clustering(colliders);
            Vector2 avoidVector = Avoidance(colliders) + ObstacleAvoidance(colliders.Length);
            Vector2 cohesionVector = Cohesion(colliders);
            Vector2 finalVector = (clusterVector + avoidVector * 2 + cohesionVector * 2);
            direction = Vector2.Lerp(direction, finalVector, strenth).normalized;
        }
        transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));
        transform.position = Warp();
        counter++;
    }

    private Vector2 Clustering(Collider2D[] colliders)
    {
        Vector2 position = transform.position;
        Vector2 clusterVector = Vector2.zero;
        foreach (Collider2D c in colliders)
        {
            if (fishDict.ContainsKey(c))
            {
                float distance = (position - (Vector2)c.transform.position).magnitude;
                clusterVector += fishDict[c].direction.normalized * Mathf.Clamp(radius / distance, 1f, 10f);
            }
        }
        return clusterVector;
    }

    private Vector2 Avoidance(Collider2D[] colliders)
    {
        Vector2 position = transform.position;
        Vector2 avoidVector = Vector2.zero;
        foreach (Collider2D c in colliders)
        {
            Vector2 fleeVector = position - (Vector2)c.transform.position;
            if (Vector2.Angle(direction, -fleeVector) <=viewAngle)
            {
                if (fishDict.ContainsKey(c))
                    avoidVector += fleeVector.normalized * Mathf.Clamp(radius / fleeVector.magnitude, 1f, 10f);
            }
        }
        return avoidVector;
    }

    private Vector2 ObstacleAvoidance(int group)
    {
        Vector2 position = transform.position;
        Vector2 avoidVector = Vector2.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius * 3f, obstacleLayer);
        foreach (Collider2D c in colliders)
        {
            Vector2 dir = (Vector2)c.transform.position - position;
            RaycastHit2D firstRay = Physics2D.Raycast(position, dir, 20, obstacleLayer);
            if (firstRay.collider != null)
            {
                RaycastHit2D secondRay = Physics2D.Raycast(position, -firstRay.normal, 20, obstacleLayer);
                avoidVector += firstRay.normal.normalized * Mathf.Clamp(radius * 3f / secondRay.distance, 1f, 100f);
            }
        }
        return avoidVector * Mathf.Log(group + 2f, 2f);
    }

    private Vector2 Cohesion(Collider2D[] colliders)
    {
        Vector2 position = transform.position;
        Vector2 massCenter = position;
        foreach (Collider2D c in colliders)
            massCenter += (Vector2)c.transform.position;
        massCenter /= (1f + colliders.Length);
        return (massCenter - position);
    }

    private Vector2 Warp()
    {
        Vector2 position = transform.position;
        if (position.x < minX)
            position = new Vector3(maxX, position.y);
        if (position.x > maxX)
            position = new Vector3(minX, position.y);
        if (position.y < minY)
            position = new Vector3(position.x, maxY);
        if (position.y > maxY)
            position = new Vector3(position.x, minY);
        return position;
    }
}

