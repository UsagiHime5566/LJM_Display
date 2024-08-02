using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureManager : MonoBehaviour
{
    public PNGLoader pNGLoader;
    public List<Transform> SpawnPoints;
    public Transform Container;
    public SpriteRenderer PrefabSignature;
    
    void Start()
    {
        pNGLoader.OnSinatureLoaded += CreateNewSignature;

        // foreach (var item in sprites)
        // {
        //     CreateNewSignature();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            //CreateNewSignature();
        }
    }

    void CreateNewSignature(Sprite sp){
        var obj = Instantiate(PrefabSignature, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, Container);
        obj.sprite = sp;

        //obj.gameObject.AddComponent<PolygonCollider2D>();
        obj.gameObject.AddComponent<CircleCollider2D>();
    }

    public void VisibleSignature(bool val){
        Container.gameObject.SetActive(val);
    }
}
