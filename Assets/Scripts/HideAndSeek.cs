using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HideAndSeek : MonoBehaviour
{
    public float totalSeekTime;
    public LayerMask seekMask;

    public Slider slider;
    private float currentSeekTime = 0;


    // Update is called once per frame
    void Update()
    {
        // Get both player objects
        // TODO: probably could just be done once one player join but it's annoying to deal with that right now
        GameObject[] hiders = {GameManager.Instance.Player1TPC?.gameObject, GameManager.Instance.Player2TPC?.gameObject}; 

        bool anySeen = false;
        Ray seekRay = new Ray();
        foreach(GameObject obj in hiders){
            if(obj == null) continue; // Deals with P2 not being in game at the start

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

        // Update slider and check for game over
        slider.value = currentSeekTime / totalSeekTime;
        if(currentSeekTime == totalSeekTime){
            GameManager.Instance.Player1IC?.Respawn();
            GameManager.Instance.Player2IC?.Respawn();
            currentSeekTime = 0;
        }
    }


    void OnEnable() {
        // Reset for new hide and seek
        currentSeekTime = 0;
        slider.value = 0;
        slider.gameObject.SetActive(true);
    }

    void OnDisable() {
        slider.gameObject.SetActive(false);
    }
}
