using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasillaCursor : MonoBehaviour
{
    public int type;
    public int x;
    public int y;
    TableroController tableroController;
    public bool isNull;

    private void Awake() {
        tableroController = FindObjectOfType<TableroController>();
    }
    public void Done() {
        isNull = false;
        Casilla m = Instantiate(tableroController.casillasPrefabs[type],Vector3.right * x,Quaternion.identity).GetComponent<Casilla>();
        m.target = this;
        ReloadName();
    }
    public void Die() {
        //Destroy(gameObject);
        //gameObject.SetActive(false);
        //Debug.Log(gameObject.name + x+y+": Muerome");
        isNull = true;
    }

    internal void MoveTo(int x, int y) {
        transform.position = new Vector3(x,y);
        this.x = x;
        this.y = y;
    }
    public void MoveDown() {
        //Destroy(gameObject);
        Debug.Log(transform.position+" mueve a "+Vector3Int.down + transform.position);
        transform.position = Vector3Int.down + transform.position;
        y = (int)transform.position.y;
    } 
    public void MoveUp() {
        //Destroy(gameObject);
        Debug.Log(transform.position+" mueve a "+Vector3Int.up + transform.position);
        transform.position = Vector3Int.up + transform.position;
        y = (int)transform.position.y;
    }

    internal void ReloadName() {
        gameObject.name += "[" + x + " " + type + " " + y + "]";
    }
    public string GetSortName() {
        return "[" + x + " " + (isNull ? "X" : type.ToString()) + " " + y + "]";
    }
    public override string ToString() {
        return this.name+" is null= "+isNull; 
    }

    private void OnMouseDown() {
        Debug.Log(GetSortName());
        tableroController.OnCasillaClickated(this);
    }
}
