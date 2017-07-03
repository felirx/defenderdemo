using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainSection
{
    public Vector2 origin { get; set; }
    public GameObject gameObject { get { return _gameObject; } }
    public Material MountainMaterial
    {
        get; set;
    }

    public float SegmentWidth
    {
        get; set;
    }

    public float SegmentStartX
    {
        get; set;
    }

    protected List<Vector2> points = null;
    protected GameObject _gameObject = null;
    protected MountainGenerator parentGenerator = null;

    protected float GroundHeight = -2;

    public void SetParentGenerator(MountainGenerator gen)
    {
        if (null == gen)
        {
            Debug.LogError("Null MountainGenerator passed to SetParentGenerator");
        }
        parentGenerator = gen;
    }

    // returns -1 on error, generated Endheight on success
    public float CreateSection(float startHeight, float endHeight, bool useEndHeight, float maxHeight)
    {
        if (null == parentGenerator)
        {
            Debug.LogError("No parent Mountain Generator assigned");
            return -1;
        }

        const int MinPoints = 1;
        const int MaxPoints = 10;
        const float allowedGrade = 60.0f;
        const float minMargin = 0.1f;
        // create points
        int pointCount = parentGenerator.Twister.Next(MinPoints, MaxPoints);
        points = new List<Vector2>(pointCount + 2);

        // add starting point
        points.Add(new Vector2(0, startHeight));

        float previousYPos = startHeight;

        // Create X positions on a line
        List<float> xPositions = new List<float>(pointCount + 2);
        xPositions.Add(0.0f); // add starting point to list
        for (int i = 0; i < pointCount; ++i)
        {
            xPositions.Add((i + 1) * (SegmentWidth / (pointCount + 2)));
        }
        xPositions.Add(SegmentWidth); // add ending point to list
        Turbulate(xPositions, minMargin, parentGenerator.Twister);

        // if we're not using pregenerated end height, generate one instead
        if (!useEndHeight)
            ++pointCount;

        // Calculate Y positions
        for (int i = 1; i < xPositions.Count; ++i)
        {
            if (useEndHeight)
            {
                // break out of the loop if we're going to create the end seam
                // will create slopes steeper then allowed for now
                if (i == xPositions.Count - 1)
                    points.Add(new Vector2(SegmentWidth, endHeight));
                continue;
            }

            float yPos = 0.0f;
            float grade = 0.0f;

            float width = xPositions[i] - xPositions[i - 1];

            yPos = parentGenerator.Twister.NextSingle(true) * maxHeight;

            // what would be the next proposed grade?
            grade = Mathf.Atan((yPos - previousYPos) / width) * Mathf.Rad2Deg;

            // 
            // allowedGrade = dY / dX -> dY = allowedGrade * dW in rads
            //
            if (Mathf.Abs(grade) > allowedGrade)
            {
                // raise or lower the y pos to minimum possible grade
                float MaxYDelta = allowedGrade * Mathf.Deg2Rad * width;
                yPos = Mathf.Clamp(previousYPos + (grade < 0.0f ? -MaxYDelta : MaxYDelta), 0.0f, maxHeight);
            }

            previousYPos = yPos;
            points.Add(new Vector2(xPositions[i], yPos));
        }

        CreateGameObject();
        return points[points.Count - 1].y;
    }

    public void Destroy()
    {
        if (_gameObject)
        {
            GameObject.Destroy(_gameObject);
            _gameObject = null;
        }
                
    }

    // Moves each point on a line left or right, but not closer to the next one than Margin states
    protected static void Turbulate(List<float> valuesOnLine, float minMargin, MersenneTwister twister)
    {
        for (int i = 1; i < valuesOnLine.Count - 1; ++i)
        {
            bool left = (twister.NextSingle() > .5f);
            if (left)
            {
                float maxPos = valuesOnLine[i - 1] + minMargin;
                float dv = twister.NextSingle() * (valuesOnLine[i] - maxPos);
                valuesOnLine[i] -= dv;
            }
            else
            {
                float maxPos = valuesOnLine[i + 1] - minMargin;
                float dv = twister.NextSingle() * (maxPos - valuesOnLine[i]);
                valuesOnLine[i] += dv;
            }
        }

    }

    protected void CreateGameObject()
    {
        _gameObject = null;
        _gameObject = new GameObject();
        _gameObject.transform.parent = parentGenerator.transform;
        _gameObject.transform.rotation = Quaternion.identity;
        _gameObject.transform.position = new Vector3(SegmentStartX, 0.0f);

        PolygonCollider2D pc2d = _gameObject.AddComponent<PolygonCollider2D>();
        _gameObject.AddComponent<RenderPolygonCollider2d>();

        // add space for 2 more corner points
        Vector2[] shape = new Vector2[points.Count + 2];

        for (int i = 0; i < points.Count; ++i)
        {
            shape[i] = new Vector2(points[i].x, points[i].y);
        }
        shape[points.Count] = new Vector2(SegmentWidth, GroundHeight);
        shape[points.Count + 1] = new Vector2(0, GroundHeight);

        pc2d.enabled = false;
        pc2d.SetPath(0, shape);
    }
}
