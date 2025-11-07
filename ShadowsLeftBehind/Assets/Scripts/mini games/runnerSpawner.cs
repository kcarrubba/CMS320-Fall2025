using UnityEngine;

public class RunnerSpawner : MonoBehaviour {
    public GameObject obstaclePrefab;
    public Transform spawnPoint;      // e.g., (12, -2, 0)
    public float spawnInterval = 1.4f;
    public int poolSize = 8;
    public Vector2 yJitter = new Vector2(-0.1f, 0.2f);

    GameObject[] pool;
    float t;

    void Start(){
        pool = new GameObject[poolSize];
        for(int i=0;i<poolSize;i++){
            pool[i] = Instantiate(obstaclePrefab);
            pool[i].SetActive(false);
        }
    }

    void Update(){
        if (!HallwayGameManager.I.alive) return;

        t += Time.deltaTime;
        if (t >= spawnInterval){
            t = 0f;
            SpawnObstacle();
        }
    }

    void SpawnObstacle(){
        var go = GetInactive();
        if (!go) return;
        var pos = spawnPoint.position;
        pos.y += Random.Range(yJitter.x, yJitter.y);
        go.transform.position = pos;
        go.SetActive(true);
    }

    GameObject GetInactive(){
        foreach (var g in pool) if (!g.activeSelf) return g;
        return null;
    }
}
