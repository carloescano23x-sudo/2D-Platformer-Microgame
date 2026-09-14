using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // Prefab used to create each rope section.
    public GameObject ropeSegmentPrefab;

    // Rigidbody2D at the bottom of the rope, usually Prototype Leg Rope.
    public Rigidbody2D connectedObject;

    // Maximum length of one rope segment.
    public float maxRopeSegmentLength = 1.0f;

    // Speed used when extending or retracting.
    public float ropeSpeed = 4.0f;

    // Current input states.
    public bool isIncreasing = false;
    public bool isDecreasing = false;

    private List<GameObject> ropeSegments = new List<GameObject>();
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (ropeSegmentPrefab == null)
        {
            Debug.LogError("Rope Segment Prefab has not been assigned.");
            return;
        }

        if (connectedObject == null)
        {
            Debug.LogError("Connected Object has not been assigned.");
            return;
        }

        ResetLength();
    }

    public void ResetLength()
    {
        foreach (GameObject segment in ropeSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }

        ropeSegments.Clear();

        isIncreasing = false;
        isDecreasing = false;

        CreateRopeSegment();
    }

    private void CreateRopeSegment()
    {
        GameObject segment = Instantiate(
            ropeSegmentPrefab,
            transform.position,
            Quaternion.identity
        );

        segment.transform.SetParent(transform, true);

        Rigidbody2D segmentBody =
            segment.GetComponent<Rigidbody2D>();

        SpringJoint2D segmentJoint =
            segment.GetComponent<SpringJoint2D>();

        if (segmentBody == null || segmentJoint == null)
        {
            Debug.LogError(
                "Rope Segment needs Rigidbody2D and SpringJoint2D."
            );

            Destroy(segment);
            return;
        }

        ropeSegments.Insert(0, segment);

        if (ropeSegments.Count == 1)
        {
            SpringJoint2D connectedJoint =
                connectedObject.GetComponent<SpringJoint2D>();

            if (connectedJoint == null)
            {
                Debug.LogError(
                    "Connected Object needs a SpringJoint2D."
                );

                return;
            }

            connectedJoint.connectedBody = segmentBody;
            connectedJoint.distance = 0.1f;

            segmentJoint.distance = maxRopeSegmentLength;
        }
        else
        {
            GameObject nextSegment = ropeSegments[1];

            SpringJoint2D nextJoint =
                nextSegment.GetComponent<SpringJoint2D>();

            nextJoint.connectedBody = segmentBody;

            segmentJoint.distance = 0.0f;
        }

        Rigidbody2D ropeBody = GetComponent<Rigidbody2D>();

        segmentJoint.connectedBody = ropeBody;
    }

    private void RemoveRopeSegment()
    {
        if (ropeSegments.Count < 2)
        {
            return;
        }

        GameObject topSegment = ropeSegments[0];
        GameObject nextSegment = ropeSegments[1];

        SpringJoint2D nextJoint =
            nextSegment.GetComponent<SpringJoint2D>();

        nextJoint.connectedBody =
            GetComponent<Rigidbody2D>();

        ropeSegments.RemoveAt(0);

        Destroy(topSegment);
    }

    void Update()
    {
        if (ropeSegments.Count == 0)
        {
            return;
        }

        GameObject topSegment = ropeSegments[0];

        SpringJoint2D topJoint =
            topSegment.GetComponent<SpringJoint2D>();

        if (isIncreasing)
        {
            if (topJoint.distance >= maxRopeSegmentLength)
            {
                CreateRopeSegment();
            }
            else
            {
                topJoint.distance += ropeSpeed * Time.deltaTime;
            }
        }

        if (isDecreasing)
        {
            if (topJoint.distance <= 0.005f)
            {
                RemoveRopeSegment();
            }
            else
            {
                topJoint.distance -= ropeSpeed * Time.deltaTime;
            }
        }

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null || connectedObject == null)
        {
            return;
        }

        lineRenderer.positionCount = ropeSegments.Count + 2;

        // Top of the rope.
        lineRenderer.SetPosition(0, transform.position);

        // Every physics segment.
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            lineRenderer.SetPosition(
                i + 1,
                ropeSegments[i].transform.position
            );
        }

        // Bottom of the rope at the gnome leg's Spring Joint anchor.
        SpringJoint2D connectedJoint =
            connectedObject.GetComponent<SpringJoint2D>();

        if (connectedJoint != null)
        {
            Vector3 bottomPosition =
                connectedObject.transform.TransformPoint(
                    connectedJoint.anchor
                );

            lineRenderer.SetPosition(
                ropeSegments.Count + 1,
                bottomPosition
            );
        }
    }

    // These methods will be useful for our UI buttons later.
    public void SetIncreasing(bool value)
    {
        isIncreasing = value;
    }

    public void SetDecreasing(bool value)
    {
        isDecreasing = value;
    }
}