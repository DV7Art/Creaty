using System.Collections.Generic;
using UnityEngine;

public class BrushPool
{
    private Stack<GameObject> pool = new Stack<GameObject>();
    private GameObject brushPrefab;
    private Transform parentTransform;

    public BrushPool(GameObject brushPrefab, int initialCount, Transform parentTransform)
    {
        this.brushPrefab = brushPrefab;
        this.parentTransform = parentTransform;

        for (int i = 0; i < initialCount; i++)
        {
            GameObject brush = Object.Instantiate(brushPrefab, parentTransform);
            brush.SetActive(false);
            pool.Push(brush);
        }
    }

    public void UpdateBrushPrefab(GameObject newBrushPrefab)
    {
        brushPrefab = newBrushPrefab;
    }

    public GameObject GetBrush()
    {
        if (pool.Count > 0)
        {
            GameObject brush = pool.Pop();
            brush.SetActive(true);
            return brush;
        }
        else
        {
            return Object.Instantiate(brushPrefab, parentTransform);
        }
    }

    public void ReturnBrush(GameObject brush)
    {
        brush.SetActive(false);
        pool.Push(brush);
    }

    public void ReturnAllBrushes()
    {
        foreach (Transform brush in parentTransform)
        {
            if (brush.gameObject.activeInHierarchy)
            {
                ReturnBrush(brush.gameObject);
            }
        }
    }
}
