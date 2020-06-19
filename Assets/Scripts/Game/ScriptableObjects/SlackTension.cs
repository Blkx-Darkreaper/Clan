using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SlackTension", menuName = "ScriptableObjects/Fishing/SlackTension")]
public class SlackTension : ScriptableObject
{
    public float Slack { get { return slack; } }
    public float DeltaSlack { get { return slackChange; } }
    public delegate void SlackChangedEventHandler(object source, EventDataArg<float> args);
    public event SlackChangedEventHandler SlackChanged;

    [ReadOnlyInInspector]
    [NonSerialized]
    protected float slack = 0;
    [NonSerialized]
    protected float slackChange = 0;

    public float Tension { get { return tension; } }
    public float DeltaTension { get { return tensionChange; } }
    public delegate void TensionChangedEventHandler(object source, EventDataArg<float> args);
    public event TensionChangedEventHandler TensionChanged;

    [ReadOnlyInInspector]
    [NonSerialized]
    protected float tension = 0;
    [NonSerialized]
    protected float tensionChange = 0;

    public void SetSlackTension(float value)
    {
        this.slack = value;
        this.tension = 0;

        if (value >= 0)
        {
            return;
        }

        this.tension = -slack;
    }

    public void ChangeSlackTension(float change)
    {
        if(change == 0)
        {
            return;
        }

        this.slack += change;
        this.slackChange = change;
        OnSlackChanged(change);

        float previousTension = tension;
        if (slack < 0)
        {
            this.tension = -slack;
        } else
        {
            this.tension = 0;
        }

        float tensionDiff = tension - previousTension;
        if(tensionDiff == 0)
        {
            return;
        }

        this.tensionChange = tensionDiff;
        OnTensionChanged(tensionDiff);
    }

    protected virtual void OnSlackChanged(float deltaSlack)
    {
        if (SlackChanged == null)
        {
            return;
        }

        SlackChanged(this, new EventDataArg<float>(deltaSlack));
    }

    protected virtual void OnTensionChanged(float deltaTension)
    {
        if(TensionChanged == null)
        {
            return;
        }

        TensionChanged(this, new EventDataArg<float>(deltaTension));
    }
}