using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;

    [SerializeField]
    protected float ropeSegmentLength = 0.25f;
    [SerializeField]
    protected float lineWidth = 0.1f;
    [SerializeField]
    protected Vector2 g = new Vector2(0f, -1f);
    [SerializeField]
    protected int totalConstraintIterations = 50;

    protected LineRenderer lineRenderer;
    protected LinkedList<RopeSegment> allRopeSegments = new LinkedList<RopeSegment>();
    private readonly float oneOverRootTwo = 1 / Mathf.Sqrt(2f);

    void Awake()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
    }

    void Start()
    {
        this.allRopeSegments.AddLast(new RopeSegment(startPosition));

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void AddSegmentToStart(Sign horizontal, Sign vertical)
    {
        Vector2 firstPosition = allRopeSegments.First.Value.currentPosition;

        if (vertical.Equals(Sign.zero) == true)
        {
            firstPosition.x += ropeSegmentLength * (int)horizontal;
        }

        if (horizontal.Equals(Sign.zero) == true)
        {
            firstPosition.y += ropeSegmentLength * (int)vertical;
        }

        if (vertical.Equals(Sign.zero) != true && horizontal.Equals(Sign.zero) != true)
        {
            firstPosition.x += oneOverRootTwo * ropeSegmentLength * (int)horizontal;
            firstPosition.y += oneOverRootTwo * ropeSegmentLength * (int)vertical;
        }

        RopeSegment ropeSegment = new RopeSegment(firstPosition);
        this.allRopeSegments.AddFirst(ropeSegment);
    }

    public void RemoveSegmentFromStart()
    {
        RopeSegment secondSegment = allRopeSegments.First.Next.Value;

        allRopeSegments.RemoveFirst();
    }

    public void AddSegmentToEnd(Sign horizontal, Sign vertical)
    {
        Vector2 lastPosition = allRopeSegments.Last.Value.currentPosition;

        if (vertical.Equals(Sign.zero) == true)
        {
            lastPosition.x += ropeSegmentLength * (int)horizontal;
        }

        if(horizontal.Equals(Sign.zero) == true)
        {
            lastPosition.y += ropeSegmentLength * (int)vertical;
        }

        if(vertical.Equals(Sign.zero) != true && horizontal.Equals(Sign.zero) != true)
        {
            lastPosition.x += oneOverRootTwo * ropeSegmentLength * (int)horizontal;
            lastPosition.y += oneOverRootTwo * ropeSegmentLength * (int)vertical;
        }

        RopeSegment ropeSegment = new RopeSegment(lastPosition);
        this.allRopeSegments.AddLast(ropeSegment);
    }

    public void RemoveSegmentFromEnd()
    {
        RopeSegment secondLastSegment = allRopeSegments.Last.Previous.Value;

        allRopeSegments.RemoveLast();
    }

    void Update()
    {
        Draw();
    }

    void FixedUpdate()
    {
        Simulate();
    }

    protected void Simulate()
    {
        // Simulation
        foreach(RopeSegment ropeSegment in allRopeSegments)
        {
            Vector2 velocity = ropeSegment.currentPosition - ropeSegment.previousPosition;

            ropeSegment.previousPosition = ropeSegment.currentPosition;
            ropeSegment.currentPosition += velocity;

            Vector2 gravity = g * Time.deltaTime;
            ropeSegment.currentPosition += gravity;
        }

        // Constraints
        for(int i = 0; i < totalConstraintIterations; i++)
        {
            ApplyConstraints();
        }
    }

    protected void ApplyConstraints()
    {
        LinkedListNode<RopeSegment> currentNode = this.allRopeSegments.First;

        // Make sure both end points are fixed
        RopeSegment firstSegment = currentNode.Value;
        firstSegment.currentPosition = startPosition;

        RopeSegment lastSegment = this.allRopeSegments.Last.Value;
        lastSegment.currentPosition = endPosition;

        if(allRopeSegments.Count < 3)
        {
            return;
        }

        // Maintain distance between points
        while(currentNode.Next != null)
        {
            RopeSegment currentSegment = currentNode.Value;
            RopeSegment nextSegment = currentNode.Next.Value;

            Vector2 delta = currentSegment.currentPosition - nextSegment.currentPosition;

            float distance = delta.magnitude;
            float error = Mathf.Abs(distance - ropeSegmentLength);

            Vector2 changeDirection = Vector2.zero;

            if(distance > ropeSegmentLength)
            {
                changeDirection = delta.normalized;
            }

            if(distance < ropeSegmentLength)
            {
                changeDirection = (nextSegment.currentPosition - currentSegment.currentPosition).normalized;
            }

            Vector2 changeAmount = changeDirection * error;

            if (currentSegment.Equals(firstSegment) == true && nextSegment.Equals(lastSegment) != true)
            {
                nextSegment.currentPosition += changeAmount;
            }

            if (currentSegment.Equals(firstSegment) != true && nextSegment.Equals(lastSegment) != true)
            {
                currentSegment.currentPosition -= changeAmount * error;
                nextSegment.currentPosition += changeAmount * 0.5f;
            }

            if(currentSegment.Equals(firstSegment) != true && nextSegment.Equals(lastSegment) == true)
            {
                firstSegment.currentPosition += changeAmount;
            }
        }
    }

    protected void Draw()
    {
        Vector3[] allRopePositions = new Vector3[this.allRopeSegments.Count];

        int i = 0;
        foreach (RopeSegment ropeSegment in allRopeSegments)
        {
            allRopePositions[i] = ropeSegment.currentPosition;
        }

        lineRenderer.positionCount = allRopeSegments.Count;
        lineRenderer.SetPositions(allRopePositions);
    }

    public enum Sign { positive = 1, zero = 0, negative = -1}

    public class RopeSegment
    {
        public Vector2 currentPosition;
        public Vector2 previousPosition;

        public RopeSegment(Vector2 position)
        {
            this.currentPosition = position;
            this.previousPosition = position;
        }
    }
}