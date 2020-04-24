using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private int initialNoRequired = 5;
    private List<BulletStruct> pooledObjects = null;

    public struct BulletStruct
    {
        public readonly GameObject bulletPrefab;
        public readonly PowerEnemyBullet bulletScript;

        public BulletStruct(GameObject o, PowerEnemyBullet bs)
        {
            bulletPrefab = o;
            bulletScript = bs;
        }
    }
    void Start()
    {  
        pooledObjects = new List<BulletStruct>();
        for (int i = 0; i < initialNoRequired; i++)
        {
            CreateNew();
        }
        
    }
    
    public BulletStruct GetPooledObject() {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].bulletPrefab.activeInHierarchy) {
                return pooledObjects[i];
            }
        }

        return CreateNew();
    }

    private BulletStruct CreateNew()
    {
         GameObject obj = Instantiate(bulletPrefab);
         obj.transform.parent = gameObject.transform;
         obj.SetActive(false);
         BulletStruct bs = new BulletStruct(obj, obj.GetComponent<PowerEnemyBullet>());
         pooledObjects.Add(bs);
         return bs;
    }
}
