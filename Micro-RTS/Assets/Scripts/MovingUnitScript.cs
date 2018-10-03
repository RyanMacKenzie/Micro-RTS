using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnitScript : MonoBehaviour {

    //Properties for each moving unit group
    [SerializeField] float unitCount;
    [SerializeField] GameObject originNode;
    [SerializeField] GameObject targetNode;
    [SerializeField] GameObject controller;
    [SerializeField] float time;
    // Use this for initialization
    void Start () {
        time = 0;
        gameObject.transform.position = originNode.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Vector3.Lerp(originNode.transform.position, targetNode.transform.position, time);
        time += .01f;
        if (time > 1)
        {
            targetNode.GetComponent<NodeScript>().UnitsInNode -= (float)unitCount;
            if(TargetNode.GetComponent<NodeScript>().UnitsInNode < 0)
            {
                targetNode.GetComponent<NodeScript>().Controller = controller;
                targetNode.GetComponent<NodeScript>().UnitsInNode = Mathf.Abs(targetNode.GetComponent<NodeScript>().UnitsInNode);
            }
            targetNode.GetComponent<NodeScript>().UnitText.GetComponent<TextMesh>().text = targetNode.GetComponent<NodeScript>().UnitsInNode.ToString();
            Destroy(gameObject);
        }
    }

    public float UnitCount
    {
        get{
            return unitCount;
        }
        set
        {
            unitCount = value;
        }
    }

    public GameObject OriginNode
    {
        get
        {
            return OriginNode;
        }
        set
        {
            originNode = value;
        }
    }

    public GameObject TargetNode
    {
        get
        {
            return targetNode;
        }
        set
        {
            targetNode = value;
        }
    }

    public GameObject Controller
    {
        get
        {
            return controller;
        }
        set
        {
            controller = value;
        }
    }
}
