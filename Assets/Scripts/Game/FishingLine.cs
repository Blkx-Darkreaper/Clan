using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    public SlackTension slackTension;

    [SerializeField]
    protected Transform rodTip;
    [SerializeField]
    protected Transform lureKnot;
    //[SerializeField]
    //protected Transform controlPoint;
    [SerializeField]
    protected float lineWidth = 0.1f;
    [SerializeField]
    protected int numPoints = 10;
    [SerializeField]
    protected float limitSlope = 10;

    protected LineRenderer lineRenderer;

    void Awake()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        this.lineRenderer.startWidth = lineWidth;
        this.lineRenderer.endWidth = lineWidth;

        UpdateControlPoint();
        Draw();

        // Subscribe to slack
        this.slackTension.SlackChanged += OnCurvatureChanged;
    }

    void OnDisable()
    {
        // Unsubscribe from slack
        this.slackTension.SlackChanged -= OnCurvatureChanged;
    }

    public void OnCurvatureChanged(object source, EventArgs args)
    {
        UpdateControlPoint();
        Draw();
    }

    public void OnLureMoved(object source, EventArgs args)
    {
        Draw();
    }

    public void UpdateControlPoint()
    {
        if(slackTension.Slack <= 0)
        {
            //this.controlPoint.position = Vector2.zero;
            transform.position = Vector2.zero;
            return;
        }

        //if (curvature == 1f)
        //{
        //    this.controlPoint = cornerPoint;
        //    return;
        //}

        Vector2 delta = lureKnot.position - rodTip.position;
        float deltaX = Mathf.Abs(delta.x);
        float deltaY = Mathf.Abs(delta.y);

        float oppositeAngle = Mathf.Atan(deltaY/deltaX);
        float quarterHypotenuse = 0.25f * Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);

        float d = Mathf.Cos(oppositeAngle) * quarterHypotenuse;

        Vector2 cornerPoint = new Vector2(rodTip.position.x, lureKnot.position.y);
        //Vector2 intersectPoint = new Vector2(rodTip.position.x + d, lure.position.y + d);
        Vector2 intersectPoint = new Vector2(cornerPoint.x + 0.25f * deltaX, cornerPoint.y + 0.75f * deltaY);

        float a = rodTip.position.x;
        float b = intersectPoint.y + limitSlope / (intersectPoint.x - a);

        float controlY = intersectPoint.y - slackTension.Slack;
        float controlX = -limitSlope / (controlY - b) + a;

        //this.controlPoint.position = new Vector2(controlX, controlY);
        transform.position = new Vector2(controlX, controlY);
    }

    //void Update()
    //{
    //    //UpdateControlPoint();   //testing
    //    Draw();
    //}

    #region Update
    protected void Draw()
    {
        Vector3[] allPoints = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)numPoints;
            Vector3 bezierPoint = CalculateLinearBezierPoint(t, rodTip.position, lureKnot.position);

            if (slackTension.Slack > 0)
            {
                //bezierPoint = CalculateQuadraticBezierPoint(t, rodTip.position, controlPoint.position, lure.position);
                bezierPoint = CalculateQuadraticBezierPoint(t, rodTip.position, transform.position, lureKnot.position);
            }

            allPoints[i] = bezierPoint;
        }

        lineRenderer.positionCount = numPoints;
        lineRenderer.SetPositions(allPoints);
    }

    protected Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        // B(t) = P0 + t(P1 – P0) = (1-t) P0 + tP1 , 0 < t < 1
        float u = 1 - t;
        Vector3 bezierPoint = u * p0 + t * p1;
        return bezierPoint;
    }

    protected Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)^2P0 + 2(1-t)tP1 + t^2P2 , 0 < t < 1
        float u = 1 - t;
        Vector3 bezierPoint = Mathf.Pow(u, 2)*p0 + 2*u*t*p1 + Mathf.Pow(t, 2)*p2;
        return bezierPoint;
    }
    #endregion Update
}