using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rope : MonoBehaviour
{
    public Rigidbody2D FirstLink { get { return allLinks.Count > 0 ? allLinks.First.Value : null; } }
    public Rigidbody2D LastLink { get { return allLinks.Count > 0 ? allLinks.Last.Value : null; } }

    [SerializeField]
    protected GameObject linkPrefab;
    [SerializeField]
    protected int linkCount = 7;
    protected LinkedList<Rigidbody2D> allLinks;
    protected Rigidbody2D hook;

    void Awake()
    {
        this.hook = GetComponent<Rigidbody2D>();

        this.allLinks = new LinkedList<Rigidbody2D>();
    }

    void Start()
    {
        if (allLinks.Count > 0)
        {
            return;
        }

        Init();
    }

    public void Init()
    {
        Rigidbody2D previousLink = hook;
        if(previousLink == null)
        {
            return;
        }

        for (int i = 0; i < linkCount; i++)
        {
            GameObject link = Instantiate(linkPrefab, hook.transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            if(joint == null)
            {
                continue;
            }

            joint.connectedBody = previousLink;

            Rigidbody2D rigidBody = link.GetComponent<Rigidbody2D>();
            if(rigidBody == null)
            {
                continue;
            }

            allLinks.AddLast(rigidBody);

            previousLink = rigidBody;
        }
    }
}