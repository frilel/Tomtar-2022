using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaggedTriggerEvent : MonoBehaviour
{
    [SerializeField]
    private bool once;
    private bool triggered = false;

    [SerializeField]
    private string[] tags;

    [SerializeField]
    private UnityEvent onTriggerEnter;
    
    [SerializeField]
    private UnityEvent onTriggerExit;

    void OnTriggerEnter(Collider other) {
        if(once && triggered) return;

        foreach(string tag in tags){
            if(other.CompareTag(tag)){
                triggered = true;
                onTriggerEnter.Invoke();
                return;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if(once && triggered) return;

        foreach(string tag in tags){
            if(other.CompareTag(tag)){
                triggered = true;
                onTriggerExit.Invoke();
                return;
            }
        }
    }
}
