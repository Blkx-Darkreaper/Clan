using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    protected LineRenderer lineRenderer;
    protected List<RopeSegment> allSegments = new List<RopeSegment>();

    public struct LineSegment
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