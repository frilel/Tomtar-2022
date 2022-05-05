using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HideAndSeek : MonoBehaviour
{
    public float totalSeekTime;
    public GameObject[] hiders;
    public LayerMask seekMask;

    public UnityEvent<bool> hiddenStatusChanged;
    public UnityEvent timeUp;
    public Slider slider;

    private bool lastAnySeen = false;
    private float currentSeekTime = 0;


    // Update is called once per frame
    void Update()
    {
        bool anySeen = false;
        Ray seekRay = new Ray();
        foreach(GameObject obj in hiders){
            seekRay.origin = obj.transform.position;
            seekRay.direction = -this.transform.up;
            if(Physics.Raycast(seekRay, out RaycastHit hit, 99f, seekMask, QueryTriggerInteraction.Collide)){
                if(hit.collider.gameObject == this.gameObject){
                    anySeen = true;
                }
            }
        }

        // Adjust seek time and keep in normal range
        if(anySeen){
            currentSeekTime += Time.deltaTime;
        } else {
            currentSeekTime -= Time.deltaTime;
        }
        currentSeekTime = Mathf.Clamp(currentSeekTime, 0, totalSeekTime);

        // Check for changes in seen state
        if(lastAnySeen != anySeen){
            hiddenStatusChanged.Invoke(anySeen);
        }
        lastAnySeen = anySeen;

        // Update slider and check for game over
        slider.value = currentSeekTime / totalSeekTime;
        if(currentSeekTime == totalSeekTime){
            timeUp.Invoke();
        }
    }
}
