using UnityEngine;
using System.Collections.Generic;
using System;

[SerializeField]
public enum eNodeType
{
    NULL, CITY, TOWN, MOUNTAIN
}

[ExecuteInEditMode]
public class Node : MonoBehaviour {
    public List<Node> neighbors;
    public eNodeType type;

    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
        if(neighbors == null)
        {
            return;
        }

        if(neighbors.Count > 0)
        {
            foreach (Node neighbor in neighbors)
            {
                if(neighbor == null)
                {
                    neighbors.Remove(neighbor);
                    break;
                }
                Color lineColor = Color.white;
                if (this.type == eNodeType.MOUNTAIN || neighbor.type == eNodeType.MOUNTAIN)
                {
                    lineColor = Color.green;
                }
                
                //Debug.DrawLine(this.transform.position, neighbor.transform.position, lineColor, 100f, true);
            }
        }
        if(type == eNodeType.CITY)
        {
            GetComponent<UISprite>().spriteName = "Button X";
        }
        else if (type == eNodeType.TOWN)
        {
            GetComponent<UISprite>().spriteName = "Checkmark";
        }
        else if (type == eNodeType.MOUNTAIN)
        {
            GetComponent<UISprite>().spriteName = "Emoticon - Dead";
        }
    }

    public void AddNode()
    {
        GameObject newObj = Instantiate(this.gameObject);
        newObj.transform.parent = this.transform.parent;
        newObj.transform.position = this.transform.position;
        newObj.transform.localScale = this.transform.localScale;
        Node node = newObj.GetComponent<Node>();
        if(node == null)
        {
            return;
        }    
        this.neighbors.Add(node);
        node.neighbors.Clear();
        node.neighbors.Add(this);
    }
}
