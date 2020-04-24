using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private int initialNoRequired = 5;
    private List<GameObject> pooledObjects = null;
    
    void Start()
    {  
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < initialNoRequired; i++)
        {
            CreateNew();
        }
        
    }
    
    public GameObject GetPooledObject() {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy) {
                return pooledObjects[i];
            }
        }

        return CreateNew();
    }

    private GameObject CreateNew()
    {
         GameObject obj = Instantiate(prefab);
         obj.transform.parent = gameObject.transform;
         obj.SetActive(false);
         pooledObjects.Add(obj);
         return obj;
    }
}
