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
    protected MountainGenerator backgroundGen = null;

    protected MersenneTwister SpawnTwister = null;
    protected PGeneric.Utilities.Timer SpawnTimer = new PGeneric.Utilities.Timer();

    protected float TimeSinceStart = 0.0f;

    protected const float BaseSpawnInterval = 1000.0f;
    protected GameObject EnemyPrefab = null;
    protected GameObject EnemyContainer = null;
    protected GameObject PlayerPrefab = null;
    protected GameObject ProjectilePrefab = null;
    protected GameObject ProjectileContainer = null;

    protected GameObject UICameraPrefab = null;
    protected GameObject UIHudPrefab = null;
    protected GameObject UIDeathScreen = null;
    protected UpdateUIScore UIScore = null;

    protected bool Initialized = false;

    // UI inputs
    public Vector2 UIInputAxis
    {
        get; set;
    }

    protected int _GameScore = 0;
    public int GameScore
    {
        get
        {
            return _GameScore;
        }
        set
        {
            _GameScore = value;
            if (UIScore)
                UIScore.UpdateScore(_GameScore);
        }
    }

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if (Initialized)
            return;

        Debug.Log("Initializing Game Manager");

        GameObject go = GameObject.Find("MountainGenerator");
        if (go)
        {
            terrainGen = go.GetComponent<MountainGenerator>();
            if (terrainGen)
            {
                // magic numbers ahoy
                terrainGen.GenerateTerrain(20, 2, 2);
                _WorldWidth = 40.0f;
            }
        }
        go = GameObject.Find("BackgroundMountains");
        if (go)
        {
            backgroundGen = go.GetComponent<MountainGenerator>();
            if (backgroundGen)
            {
                // magic numbers ahoy
                backgroundGen.GenerateTerrain(40, 2, 10);
            }
        }

        EnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        if (null == EnemyPrefab)
        {
            Debug.LogError("Could not load Enemy Prefab");
        }

        PlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        if (null == PlayerPrefab)
        {
            Debug.LogError("Could not load Player Prefab");
            return;
        }
        go = GameObject.Instantiate<GameObject>(PlayerPrefab, Vector3.up * 3.0f, Quaternion.identity);
        if (null == go)
        {
            Debug.LogError("Could not instantiate Player Prefab");
            return;
        }
        _player = go.GetComponent<Player>();
        if (null == MyPlayer)
        {
            Debug.LogError("Could not find player component");
            return;
        }

        ProjectilePrefab = Resources.Load<GameObject>("Prefabs/Projectile");
        if (null == ProjectilePrefab)
        {
            Debug.LogError("Could not load Projectile Prefab");
        }

        // parent the camera
        Camera.main.transform.parent = go.transform;
        Camera.main.transform.localRotation = Quaternion.identity;
        Camera.main.transform.localPosition = new Vector3(0.0f, 1.0f, -10.0f);

        // reset score
        GameScore = 0;

        LoadUI();

        Initialized = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (null == MyPlayer)
            return;

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

    public void PlayerDeath()
    {
        // deparent camera
        Camera.main.transform.parent = null;

        GameObject.Destroy(MyPlayer.gameObject);
        _player = null; // stops enemies from moving and spawning

        if (UIDeathScreen)
            UIDeathScreen.SetActive(true);
    }

    public void SpawnProjectile(GameObject owner, Vector3 pos, Vector3 dir, float vel, float life)
    {
        GameObject go = GameObject.Instantiate<GameObject>(ProjectilePrefab, pos, Quaternion.identity);
        if (null == go)
        {
            Debug.LogError("Could not instantiate projectile prefab");
            return;
        }
        Projectile p = go.GetComponent<Projectile>();
        if (null == p)
        {
            Debug.LogError("Could not get projectile component");
            GameObject.Destroy(go);
            return;
        }

        if (null == ProjectileContainer)
            ProjectileContainer = new GameObject("ProjectileContainer");

        p.gameObject.transform.parent = ProjectileContainer.transform;
        p.gameObject.layer = owner.layer;
        p.Direction = dir;
        p.Velocity = vel;
        p.LifeTime = life;
    }

    protected void LoadUI()
    {
        UICameraPrefab = Resources.Load<GameObject>("Prefabs/UI/UICamera");
        if (null == UICameraPrefab)
        {
            Debug.LogError("Could not load UICamera Prefab");
            return;
        }

        UIHudPrefab = Resources.Load<GameObject>("Prefabs/UI/Hud");
        if (null == UIHudPrefab)
        {
            Debug.LogError("Could not load UIHud Prefab");
            return;
        }

        GameObject uiCam = GameObject.Instantiate(UICameraPrefab);
        if (!uiCam)
            return;

        Transform canvas = PGeneric.Utilities.FindUtilities.SearchHierarchyForTransform(uiCam.transform, "Canvas");
        if (!canvas)
            return;

        GameObject.Instantiate(UIHudPrefab, canvas);

        Transform t = PGeneric.Utilities.FindUtilities.SearchHierarchyForTransform(uiCam.transform, "DeathContainer");
        if (t)
            UIDeathScreen = t.gameObject;

        UIScore = PGeneric.Utilities.FindUtilities.SearchHierarchyForComponent<UpdateUIScore>(uiCam.transform, "ScoreText");
    }

    protected void HandleEnemySpawns()
    {
        if (null == EnemyPrefab)
            return;

        if (null == _player)
            return;

        if (null == SpawnTwister)
        {
            SpawnTwister = new MersenneTwister(Random.Range(int.MinValue, int.MaxValue));
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
            // reject spawn points that are too close to the player
            float DistToPlayer = 0.0f;
            Vector3 proposedSpawnPoint = Vector3.zero;
            do
            {
                proposedSpawnPoint = new Vector3(-WorldWidth * 0.5f + SpawnTwister.NextSingle() * WorldWidth, SpawnTwister.NextSingle() * WorldHeight, 0.0f);
                DistToPlayer = Vector3.Distance(MyPlayer.transform.position, proposedSpawnPoint);
            } while (DistToPlayer < 5.0f);

            GameObject.Instantiate(EnemyPrefab, proposedSpawnPoint, Quaternion.identity, EnemyContainer.transform);

            // increase spawn rate until enough time has passed, should be pretty crazy
            float enduranceFactor = 1.0f - Mathf.Clamp(TimeSinceStart / (2.0f * 60.0f * 1000.0f), 0.0f, 0.8f);
            SpawnTimer.Start((BaseSpawnInterval + BaseSpawnInterval * SpawnTwister.NextSingle()) * enduranceFactor);
        }
    }
}
