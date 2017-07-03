using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    protected int Segments = 20;
    protected float SegmentWidth = 2;
    protected float MaxHeight = 2;

    protected float PlayerPosition = 0.0f;
    protected float PrevPlayerPos = 0.0f;

    public Material MountainMaterial = null;

    public MersenneTwister Twister { get { return _twister; } }
    protected MersenneTwister _twister = null;
    List<MountainSection> Mountains = new List<MountainSection>();

    protected float LeftEdge = 0;
    protected float RightEdge = 0; 

    public void GenerateTerrain(int segments, int segmentWidth, int maxHeight)
    {
        Segments = segments;
        SegmentWidth = segmentWidth;
        MaxHeight = maxHeight;

        GenerateContinuousMountains();
        MToolBox.GM.OnWorldWrappingUpdate += HandleTerrainWrapping;
    }

    void OnDestroy()
    {
        MToolBox.GM.OnWorldWrappingUpdate -= HandleTerrainWrapping;
        for (int i = 0; i < Mountains.Count; ++i)
        {
            if (null != Mountains[i])
                Mountains[i].Destroy();
        }

    }

    protected void WorldReorigin()
    {
        PrevPlayerPos = MToolBox.GM.MyPlayer.transform.position.x;
    }

    protected void GenerateContinuousMountains()
    {
        int twisterSeed = Random.Range(int.MinValue, int.MaxValue);
        _twister = new MersenneTwister(twisterSeed);
        float currentHeight = -1.0f;
        float startHeight = Twister.NextSingle() * MaxHeight;

        if (Mountains.Count != 0)
        {
            for (int i = 0; i < Mountains.Count; ++i)
            {
                if (null == Mountains[i])
                    continue;
                Mountains[i].Destroy();
            }
            Mountains.Clear();
        }

        for (int i = 0; i < Segments; ++i)
        {
            MountainSection section = new MountainSection();
            section.SetParentGenerator(this);
            section.SegmentWidth = SegmentWidth;
            section.SegmentStartX = (-SegmentWidth * (Segments / 2)) + i * SegmentWidth;
            if (i == 0)
                currentHeight = section.CreateSection(startHeight, -1, false, MaxHeight);
            else if (i < (Segments - 1))
                currentHeight = section.CreateSection(currentHeight, -1, false, MaxHeight);
            else
                currentHeight = section.CreateSection(currentHeight, startHeight, true, MaxHeight);

            Mountains.Add(section);

            MToolBox.IM.RegisterTerrain(section.gameObject);
        }
    }

    protected void HandleTerrainWrapping(Vector2 focus, float maxDistance, bool left)
    {
        if (Mountains.Count < 3)
        {
            Debug.LogError("Too few segments to work with");
            return;
        }
        int indexToCheck = left ? (Mountains.Count - 1) : 0;

        for (;;)
        {
            if (DistanceCheck(Mountains[indexToCheck], focus, maxDistance))
            {
                WrapSection(indexToCheck, left);
            }
            else
            {
                break;
            }
        }
    }

    protected bool DistanceCheck(MountainSection ms, Vector2 focus, float MaxDistance)
    {
        float dist = Mathf.Abs(ms.gameObject.transform.position.x - focus.x);
        return MaxDistance < dist;
    }

    protected void WrapSection(int index, bool left)
    {
        // really lazy way of handling the data...
        MountainSection ms = Mountains[index];
        MountainSection newNeighbor = null;
        Mountains.RemoveAt(index);
        if (left)
        {
            Mountains.Insert(0, ms);
            newNeighbor = Mountains[1];
        }
        else
        {
            Mountains.Add(ms);
            newNeighbor = Mountains[Mountains.Count - 2];
        }
 
        Vector3 pos = ms.gameObject.transform.position;
        ms.gameObject.transform.position = new Vector3(newNeighbor.gameObject.transform.position.x + (left ? -SegmentWidth : SegmentWidth), pos.y, pos.z);
    }
}
