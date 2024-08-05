using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureManager : MonoBehaviour
{
    [Header("元件設定")]
    public PNGLoader pNGLoader;
    public List<Transform> SpawnPoints;
    public Transform Container;
    public SpriteRenderer PrefabSignature;

    [Header("參數設定")]
    public int maxSignature = 80;
    
    void Start()
    {
        pNGLoader.OnSinatureLoaded += CreateNewSignature;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Q)){
            PageLeft();
        }
        if(Input.GetKeyDown(KeyCode.E)){
            PageRight();
        }
    }

    void CreateNewSignature(Sprite sp, System.DateTime date){
        var obj = Instantiate(PrefabSignature, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, Container);
        obj.sprite = sp;

        //obj.gameObject.AddComponent<PolygonCollider2D>();
        obj.gameObject.AddComponent<CircleCollider2D>();


    }

    public void VisibleSignature(bool val){
        Container.gameObject.SetActive(val);
    }

    public void PageLeft(){

    }

    public void PageRight(){
        
    }
}
