using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour {
    public int type;
    public int x;
    public int y;
    public CasillaCursor target;
    public float correctionFactor = 0.1f;
    Animator animator;
    string anim;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (target) {
            Vector2 error = target.transform.position - transform.position;
            transform.position += (Vector3)error * correctionFactor;
            //   gameObject.SetActive(target.gameObject.activeSelf);
            if (target.isNull) {
                if (animator.enabled) {
                    anim = "Die";
                    
                    Invoke(nameof(StartAnimation), UnityEngine.Random.Range(0,0.5f));
                    Invoke(nameof(Hide), 1f);
                }
            }
        }
    }

    internal void ReloadName() {
        gameObject.name += "[" + x + " " + type + " " + y + "]";
    }
    private void OnMouseOver() {
        Debug.Log(name);
    }
    private void StartAnimation() {
        switch (anim) {
            case "Die":
                animator.Play(anim);
                break;
            default:
                break;
        }
        anim = "";
    } 
    private void Hide() {
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
