using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public static int[] skillsTotal = new int[4];
    public int foodSkill = 0;
    public int attackSkill = 1;
    public int defSkill = 0;
    public float energy = 10;
    public float age = 0;
    public GameObject prefab;

    private NN nn;

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

        // значения на вход нейросети
        for (int i = 0; i < 4; i++)
        {
            if (neighboursCount[i] > 0)
            {
                vectors[i] /= neighboursCount[i] * vision;
                inputs[i] = vectors[i].magnitude;
            }
            else
            {
                inputs[i] = 0f;
            }
        }

        // значения выхода нейросети
        float[] outputs = nn.FeedForward(inputs);
        Vector2 target = new Vector2(0, 0);
        for (int i = 0; i < 4; i++)
        {
            if (neighboursCount[i] > 0)
            {
                Vector2 dir = new Vector2(vectors[i].x, vectors[i].y);
                dir.Normalize();
                target += dir * outputs[i];
            }
        }
        if (target.magnitude > 1f) target.Normalize();
        Vector2 velocity = rb.velocity;
        velocity += target * (0.25f + attackSkill * 0.05f);
        velocity *= 0.98f;
        rb.velocity = velocity;

        // уменьшаем энергию бактерии
        energy -= Time.deltaTime / 2f;
        if (energy < 0f)
        {
            Kill();
        }
    }

    public void Init(Genome g)
    {
        genome = g;
        Color col = new Color(1f, 1f, 1f, 1f);

        float size = 0.5f;
        for (int i = 0; i < Genome.skillCount; i++)
        {
            skillsTotal[g.skills[i]]++;
            if (g.skills[i] == 0)
            {
                foodSkill++;
                col.g -= 0.3f;
            }
            else if (g.skills[i] == 1)
            {
                attackSkill++;
                col.r -= 0.25f;
            }
            else if (g.skills[i] == 2)
            {
                defSkill++;
                col.b -= 0.25f;
            }
            else if (g.skills[i] == 3)
            {
                size += 0.1f;
            }
        }
        transform.localScale = new Vector3(size, size, size);
        gameObject.GetComponent<SpriteRenderer>().color = col;

        nn = new NN(inputsCount, 8, 4);
        for (int i = 0; i < inputsCount; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                nn.layers[0].weights[i, j] = genome.weights[i + j * inputsCount];
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                nn.layers[1].weights[i, j] = genome.weights[i + j * 8 + inputsCount * 8];
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (foodSkill == 0) return;
        if (col.gameObject.name == "food")
        {
            Eat(foodSkill);
            Destroy(col.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (attackSkill == 0) return;
        if (collision.gameObject.name == "bacterium")
        {
            // рассчитываем урон
            AI ai = collision.gameObject.GetComponent<AI>();
            if (ai.age < 1f) return;
            float damage = Mathf.Max(0f, attackSkill - ai.defSkill);
            float foodFromBacteria = ai.energy / 1.25f;
            float expirience = 1.25f;
            ai.energy -= damage * 5f;
            if (ai.energy == 0f)
            {
                ai.Kill();
                attackSkill += expirience;
                Eat(foodFromBacteria);
            }
        }
    }

    private void Eat(float foodWeight)
    {
        energy += foodWeight;
        // размножение
        if (energy > 100)
        {
            energy *= 0.1f;
            GameObject bacterium = (GameObject)Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            bacterium.transform.position = transform.position;
            bacterium.name = "bacterium";
            Genome g = new Genome(genome);
            // мутация генома
            g.Mutate(0.5f);
            AI ai = bacterium.GetComponent<AI>();
            ai.Init(g);
            ai.energy = energy * 1.25f;
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

}
