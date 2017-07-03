using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected float ResetDistance = 10.0f;
    protected Player _player = null;

    public Player MyPlayer { get { return _player; } }

    public delegate void WorldWrappingUpdate(Vector2 focus, float maxDistance, bool left);
    public WorldWrappingUpdate OnWorldWrappingUpdate;

    protected float _WorldWidth = 40.0f;
    public float WorldWidth { get { return _WorldWidth; } }

    protected float _WorldHeight = 10.0f;
    public float WorldHeight { get { return _WorldHeight; } }

    protected MountainGenerator terrainGen = null;

    protected MersenneTwister SpawnTwister = null;
    protected PGeneric.Utilities.Timer SpawnTimer = new PGeneric.Utilities.Timer();

    protected float TimeSinceStart = 0.0f;

    protected const float BaseSpawnInterval = 1000.0f;
    protected GameObject EnemyPrefab = null;
    protected GameObject EnemyContainer = null;

    public void Init()
    {
        Debug.Log("Initializing Game Manager");

        terrainGen = FindObjectOfType<MountainGenerator>();
        if (terrainGen)
        {
            // magic numbers ahoy
            terrainGen.GenerateTerrain(20, 2, 2);
            _WorldWidth = 40.0f;
        }

        EnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        if (null == EnemyPrefab)
        {
            Debug.LogError("Could not load Enemy Prefab");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (null == MyPlayer)
        {
            // shit solution
            _player = FindObjectOfType<Player>();
            if (null == MyPlayer)
            {
                Debug.LogError("Could not find player");
                return;
            }
        }

        // did the player go past reset distance?
        Vector3 playerX = new Vector3(MyPlayer.transform.position.x, 0.0f, 0.0f);
        if (Vector3.Distance(Vector3.zero, playerX) > ResetDistance)
        {
            ResetWorldCenter.ResetWorldToOrigin(playerX);
        }

        // handle world looping
        Vector2 focusPoint = MyPlayer.transform.position;

        Vector3 playerVelocity = MyPlayer.rigidBody.velocity;
        if (playerVelocity.magnitude > 0.05f)
        {
            // figure out player movement direction
            bool left = Vector3.Cross(playerVelocity, Vector3.up).z < 0.0f;
            OnWorldWrappingUpdate.Invoke(focusPoint, WorldWidth * 0.5f, left);
        }

        HandleEnemySpawns();
    }

    protected void HandleEnemySpawns()
    {
        if (null == EnemyPrefab)
            return;

        if (null == SpawnTwister)
        {
            SpawnTwister = new MersenneTwister(14);
            TimeSinceStart = 0.0f;
        }

        if (null == EnemyContainer)
            EnemyContainer = new GameObject("EnemyContainer");

        if (!SpawnTimer.Started())
        {
            SpawnTimer.Start(100.0f);
        }
        TimeSinceStart += Time.deltaTime;

        if (SpawnTimer.Tick(Time.deltaTime))
        {
            GameObject go = GameObject.Instantiate(EnemyPrefab, new Vector3(-WorldWidth * 0.5f + SpawnTwister.NextSingle() * WorldWidth, SpawnTwister.NextSingle() * WorldHeight, 0.0f), Quaternion.identity, EnemyContainer.transform);

            // increase spawn rate until enough time has passed, should be pretty crazy
            float enduranceFactor = 1.0f - Mathf.Clamp(TimeSinceStart / (5.0f * 60.0f * 10000.0f), 0.0f, 0.8f);
            Debug.Log(enduranceFactor);
            SpawnTimer.Start((BaseSpawnInterval + BaseSpawnInterval * SpawnTwister.NextSingle()) * enduranceFactor);
        }
    }
}
