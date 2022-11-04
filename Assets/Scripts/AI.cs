using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public static int[] skillsTotal = new int[4];
    public int foodSkill = 0;
    public int attackSkill = 0;
    public int defSkill = 0;
    public float energy = 10;
    public float age = 0;

    // test
    public float[] neighboursCount = new float[4];
    public Vector3[] vectors = new Vector3[4];
    //
    private int inputsCount = 4;
    private Genome genome;

    private Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90);
        age += Time.deltaTime;
    }

    void FixedUpdate()
    {
        // область видимости бактерии
        float vision = 1f + (attackSkill + defSkill + foodSkill) / 3;
        float[] inputs = new float[inputsCount];

        // получаем общее кол-во объектов вокруг
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vision);

        // векторы к центрам масс еды, красного, зеленого и синего
        
        for (int i = 0; i < 4; i++)
        {
            neighboursCount[i] = 0;
            vectors[i] = new Vector3(0f, 0f, 0f);
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject == gameObject) continue;
            if (colliders[i].gameObject.name == "food")
            {
                neighboursCount[0]++;
                vectors[0] += colliders[i].gameObject.transform.position - transform.position;
            }
            else if (colliders[i].gameObject.name == "bacterium")
            {
                AI ai = colliders[i].gameObject.GetComponent<AI>();
                neighboursCount[1] += ai.attackSkill / 3f;
                vectors[1] += (colliders[i].gameObject.transform.position - transform.position) * ai.attackSkill;
                neighboursCount[2] += ai.foodSkill / 3f;
                vectors[2] += (colliders[i].gameObject.transform.position - transform.position) * ai.foodSkill;
                neighboursCount[3] += ai.defSkill / 3f;
                vectors[3] += (colliders[i].gameObject.transform.position - transform.position) * ai.defSkill;
            }
        }

        if (energy < 0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        for (int i = 0; i < Genome.skillCount; i++)
        {
            skillsTotal[genome.skills[i]]--;
        }
        Destroy(gameObject);
    }

}
