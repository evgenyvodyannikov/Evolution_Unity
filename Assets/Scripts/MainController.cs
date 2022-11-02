using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public Vector2 area = new Vector2(10f, 10f);

    public GameObject bacteriumPrefab;
    public GameObject foodPrefab;

    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
         Evolution();
    }

    private void Evolution()
    {
        // Start evolution
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
