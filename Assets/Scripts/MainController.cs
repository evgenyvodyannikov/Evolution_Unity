using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public Vector2 area = new Vector2(10f, 4.5f);

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
        // spawn food
        for (int i = 0; i < 250; i++)
        {
            GameObject food = Instantiate(foodPrefab, new Vector3(Random.Range(-area.x, area.x), Random.Range(-area.y, area.y), 0.1f), Quaternion.identity);
            food.name = "food";
        }

        // spawn bacteria
        for (int i = 0; i < 50; i++)
        {
            Genome genome = new Genome(64);
            GameObject bacterium = Instantiate(bacteriumPrefab, new Vector3(Random.Range(-area.x, area.x), Random.Range(-area.y, area.y), 0), Quaternion.identity);
            bacterium.name = "bacterium";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
