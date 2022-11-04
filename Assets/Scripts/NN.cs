using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN
{
    public Layer[] layers;

    public NN(params int[] sizes)
    {
        layers = new Layer[sizes.Length];
        for (int i = 0; i < sizes.Length; i++)
        {
            int nextSize = 0;
            if(i < sizes.Length - 1) nextSize = sizes[i + 1];
            layers[i] = new Layer(sizes[i], nextSize);
            for (int j = 0; j < sizes[i]; j++)
            {
                for (int k = 0; k < nextSize; k++)
                {
                    layers[i].weights[j, k] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

}