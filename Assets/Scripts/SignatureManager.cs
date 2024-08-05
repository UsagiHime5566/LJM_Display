using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureManager : MonoBehaviour
{
    [Header("元件設定")]
    public PNGLoader pNGLoader;
    public SpriteRenderer PrefabSignature;
    public List<Transform> SpawnPoints;
    public Transform Container;

    [Header("Runtime")]
    public List<Transform> ChildPool;
    public Transform createPool;
    public int viewPool;
    

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
        CheckPool();

        var obj = Instantiate(PrefabSignature, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, createPool);
        obj.sprite = sp;
        obj.name = $"{date}";

        //obj.gameObject.AddComponent<PolygonCollider2D>();
        obj.gameObject.AddComponent<CircleCollider2D>();
    }

    void CheckPool(){
        if(ChildPool.Count == 0){
            ChildPool = new List<Transform>();
            var newPool = new GameObject($"Pool {ChildPool.Count}").transform;
            newPool.SetParent(Container);
            newPool.gameObject.SetActive(false);
            ChildPool.Add(newPool);
            createPool = newPool;
        } else {
            if(createPool.childCount > maxSignature){
                var newPool = new GameObject($"Pool {ChildPool.Count}").transform;
                newPool.SetParent(Container);
                newPool.gameObject.SetActive(false);
                ChildPool.Add(newPool);
                createPool = newPool;
            }
        }
    }

    public void VisibleSignature(bool val){
        Container.gameObject.SetActive(val);

        if(val){
            viewPool = ChildPool.Count - 1;
            ChildPool[viewPool].gameObject.SetActive(true);
        } else {
            ChildPool[viewPool].gameObject.SetActive(false);
        }
    }

    public void PageLeft(){
        if(!Container.gameObject.activeSelf)
            return;

        ChildPool[viewPool].gameObject.SetActive(false);
        viewPool = Mathf.Min(ChildPool.Count - 1, viewPool + 1);
        ChildPool[viewPool].gameObject.SetActive(true);
    }

    public void PageRight(){
        if(!Container.gameObject.activeSelf)
            return;
            
        ChildPool[viewPool].gameObject.SetActive(false);
        viewPool = Mathf.Max(0, viewPool - 1);
        ChildPool[viewPool].gameObject.SetActive(true);
    }
}
