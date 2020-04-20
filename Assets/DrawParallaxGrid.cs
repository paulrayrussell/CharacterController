using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParallaxGrid : MonoBehaviour
{

    [SerializeField] private float columnsX = 15f;
    [SerializeField] private float rowsY = 3f;
    // [SerializeField] private GameObject scrollPrefab = null;
    [SerializeField] private GameObject staticPrefab = null;
    // [SerializeField] private GameObject scrollLayer = null;
    [SerializeField] private GameObject staticLayer = null;
    
    private void Start()
    {
        
        float extentsX = (staticPrefab.GetComponent<SpriteRenderer>().bounds.extents.x);

        float xPixs = columnsX * extentsX * 2;
        float yPixs = rowsY * extentsX * 2;
        var camOrigin = new Vector2(transform.position.x + extentsX, transform.position.y + extentsX);
        
        for (float y = 0; y < yPixs; y += (extentsX * 2))
        {
            for (float x = 0; x < xPixs; x += (extentsX * 2))
            {
                // GameObject scrollPiece = Instantiate(scrollPrefab, new Vector2(camOrigin.x + x, camOrigin.y + y), Quaternion.identity);
                // scrollPiece.transform.SetParent(scrollLayer.transform);
                GameObject staticPiece = Instantiate(staticPrefab, new Vector2(camOrigin.x + x, camOrigin.y + y), Quaternion.identity);
                staticPiece.transform.SetParent(staticLayer.transform);
            }
        }
        
   
        
    }

    
}
