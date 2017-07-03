using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    protected int Segments = 20;
    protected float SegmentWidth = 2;
    protected float MaxHeight = 5;

    public Material MountainMaterial = null;

    public MersenneTwister Twister { get { return _twister; } }
    protected MersenneTwister _twister = null;
    List<MountainSection> Mountains = new List<MountainSection>();

    // Use this for initialization
    void Awake()
    {
        GenerateContinuousMountains();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        for (int i = 0; i < Mountains.Count; ++i)
        {
            if (null != Mountains[i])
                Mountains[i].Destroy();
        }

    }

    protected void GenerateContinuousMountains()
    {
        int twisterSeed = 14;
        _twister = new MersenneTwister(twisterSeed);
        // hardcoded for now
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
            section.SegmentStartX = i * SegmentWidth;
            if (i == 0)
                currentHeight = section.CreateSection(startHeight, -1, false, MaxHeight);
            else if (i < (Segments - 1))
                currentHeight = section.CreateSection(currentHeight, -1, false, MaxHeight);
            else
                currentHeight = section.CreateSection(currentHeight, startHeight, true, MaxHeight);
        }
    }
}
