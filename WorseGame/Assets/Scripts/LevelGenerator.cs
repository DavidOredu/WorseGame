using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    private GameObject player;

    public List<ObjectPooler.PoolTag> poolTags;

    Vector3 spawnPoint = new Vector3();
    public Vector3 spawnArea;
    public Vector3 areaOffset;

    public List<GameObject> boxesSpawned = new List<GameObject>();

    public float playerMaxHeight = 600f;
    public float boxMinDist;
    public float boxMaxDist;
    public float boxFarDist;
    // Start is called before the first frame update
    void Start()
    {
        poolTags = Resources.Load<SpawnableBoxes>("SpawnableBoxes").poolTags;
        player = GameObject.FindGameObjectWithTag("Player");
        Spawn(spawnPoint, spawnArea);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var box in boxesSpawned)
        {
            var dist = Vector3.Distance(box.transform.position, player.transform.position);
            if (dist > boxMaxDist && player.transform.position.y < playerMaxHeight)
            {
                UpdateBoxPositions(box, true);
            }
        }
    }
    void Spawn(Vector3 spawnPoint, Vector3 spawnArea)
    {
        var poolDictionary = ObjectPooler.instance.poolDictionary;
        for (int i = 0; i < poolTags.Count; i++)
        {
            foreach (var obj in poolDictionary[poolTags[i]])
            {
                boxesSpawned.Add(obj);
            }
        }

        int sizeOfBox = ObjectPooler.instance.GetSizeWithTag(ObjectPooler.PoolTag.Normal);
        for (int i = 0; i < sizeOfBox; i++)
        {
            for (int x = 0; x < sizeOfBox; x++)
            {
                ObjectPooler.instance.SpawnFromPool(ObjectPooler.PoolTag.Normal, new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x) + spawnPoint.x,
                Random.Range(-spawnArea.y, spawnArea.y) + spawnPoint.y,
                Random.Range(-spawnArea.z, spawnArea.z) + spawnPoint.z),
                Quaternion.identity);
            }
        }
    }

    void UpdateBoxPositions(GameObject box, bool isFar = false)
    {
        if(!(ScoreSystem.GameScore.score >= GameManager.instance.GetPercentageOfExperienceToNextLevel(box.GetComponent<Box>().boxData.percentageToSpawn))) { return; }
        box.SetActive(true);
        var boxScript = box.GetComponent<Box>();
        if(boxScript.boxType == Box.BoxType.Kamikaze)
        {
            var kamikaze = box.GetComponent<KamikazeEnemy>();
            kamikaze.currentKamikazeState = KamikazeEnemy.EnemyState.Idle;
            kamikaze.trail1.enabled = false;
            kamikaze.trail2.enabled = false;
        }
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        float dist;

        if (isFar)
        {
            dist = Random.Range(boxFarDist, boxMaxDist);
        }
        else
        {
            dist = Random.Range(boxMinDist, boxMaxDist);
        }
        Vector3 newPos = player.transform.position + (dir * dist);

        if (isFar && Vector3.Distance(newPos, player.transform.position) < boxFarDist)
        {
            UpdateBoxPositions(box, isFar);
            return;
        }
        if (Physics2D.OverlapCircle(newPos, 10f, LayerMask.GetMask("Box", "Ground", "LavaPit")))
        {
            UpdateBoxPositions(box, isFar);
            return;
        }
        box.transform.position = newPos;
        box.transform.eulerAngles = Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //    Gizmos.DrawCube(new Vector3(player.transform.position.x + areaOffset.x, transform.position.y + areaOffset.y, player.transform.position.z + areaOffset.z), spawnArea);
    }
}
