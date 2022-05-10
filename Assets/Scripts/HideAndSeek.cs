using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HideAndSeek : MonoBehaviour
{
    [SerializeField]
    private float totalSeekTime;

    [SerializeField]
    private LayerMask seekMask;

    [SerializeField]
    private Slider slider;
    
    private BoxCollider[] seekPlanes;
    private float currentSeekTime = 0;

    void Start(){
        seekPlanes = GetComponentsInChildren<BoxCollider>();
    }


    // Update is called once per frame
    void Update()
    {
        // Get both player objects
        // TODO: probably could just be done once one player join but it's annoying to deal with that right now
        GameObject[] hiders = {
            GameManager.Instance.Player1TPC?.transform.Find("PlayerCameraRoot").gameObject,
            GameManager.Instance.Player2TPC?.transform.Find("PlayerCameraRoot").gameObject,
        };

        bool anySeen = false;
        Ray seekRay = new Ray();
        foreach(BoxCollider box in seekPlanes){
            if(!box.gameObject.activeInHierarchy) continue;

            foreach(GameObject obj in hiders){
                if(obj == null) continue; // Deals with P2 not being in game at the start

                seekRay.origin = obj.transform.position;
                seekRay.direction = -box.transform.up;
                if(!Physics.Raycast(seekRay, out RaycastHit hit, 99f, seekMask, QueryTriggerInteraction.Collide)) continue;
                
                Debug.DrawLine(seekRay.origin, hit.point, Color.red, Time.deltaTime);
                if(hit.collider == box){
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
