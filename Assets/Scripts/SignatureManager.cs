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
    public float signCircle = 3.75f;
    
    void Start()
    {
        pNGLoader.OnSinatureLoaded += CreateNewSignature;
        CheckPool();
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
        obj.gameObject.AddComponent<CircleCollider2D>().radius = signCircle;
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
            if(createPool.childCount >= maxSignature){
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
            CheckArrowUsage();
            ChildPool[viewPool].gameObject.SetActive(true);
        } else {
            ChildPool[viewPool].gameObject.SetActive(false);
        }
    }

    public void PageLeft(){
        if(!Container.gameObject.activeSelf)
            return;

        ChildPool[viewPool].gameObject.SetActive(false);
        //viewPool = viewPool + 1 >= ChildPool.Count ? 0 : viewPool + 1;
        viewPool = Mathf.Min(ChildPool.Count - 1, viewPool + 1);
        CheckArrowUsage();
        ChildPool[viewPool].gameObject.SetActive(true);
    }

    public void PageRight(){
        if(!Container.gameObject.activeSelf)
            return;

        ChildPool[viewPool].gameObject.SetActive(false);
        //viewPool = viewPool - 1 < 0  ? ChildPool.Count - 1 : viewPool - 1;
        viewPool = Mathf.Max(0, viewPool - 1);
        CheckArrowUsage();
        ChildPool[viewPool].gameObject.SetActive(true);
    }

    public void CheckArrowUsage(){
        //起點
        if(viewPool == ChildPool.Count - 1){
            ESNetwork.instance.SendLeftUsage(0);

            //右邊有東西
            if(ChildPool.Count > 1){
                ESNetwork.instance.SendRightUsage(1);
            } else {
                ESNetwork.instance.SendRightUsage(0);
            }
        }
        //最右邊
        else if(viewPool == 0){
            ESNetwork.instance.SendRightUsage(0);

            //左邊有東西
            if(ChildPool.Count > 1){
                ESNetwork.instance.SendLeftUsage(1);
            } else {
                ESNetwork.instance.SendLeftUsage(0);
            }
        } else {
            ESNetwork.instance.SendRightUsage(1);
            ESNetwork.instance.SendLeftUsage(1);
        }
    }
}
