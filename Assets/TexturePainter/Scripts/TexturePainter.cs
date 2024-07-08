using UnityEngine;
using System.Collections;
using System.IO;

public class TexturePainter : MonoBehaviour
{
    public GameObject brushCursor, brushContainer;
    public Camera sceneCamera, canvasCam;
    public Sprite cursorPaint;
    public RenderTexture canvasTexture;
    public Material baseMaterial;
    public int initialBrushCount = 100;

    private float brushSize = 1.0f;
    private float brushIntensity = 1.0f;
    private Color brushColor;
    private int brushCounter = 0, MAX_BRUSH_COUNT = 1000;
    private bool saving = false;
    private string currentBrush = "TexturePainter-Instances/BrushEntity";

    private BrushPool brushPool;

    void Start()
    {
        GameObject brushPrefab = Resources.Load<GameObject>(currentBrush);
        brushPool = new BrushPool(brushPrefab, initialBrushCount, brushContainer.transform);
    }

    void Update()
    {
        brushColor = ColorSelector.GetColor();
        if (Input.GetMouseButton(0))
        {
            DoAction();
        }
        UpdateBrushCursor();
    }

    void DoAction()
    {
        if (saving) return;

        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            GameObject brushObj = brushPool.GetBrush();
            brushObj.GetComponent<SpriteRenderer>().color = brushColor;
            brushColor.a = brushSize * 2.0f;
            brushObj.transform.localPosition = uvWorldPosition;
            brushObj.transform.localScale = Vector3.one * brushSize;
        }

        brushCounter++;
        if (brushCounter >= MAX_BRUSH_COUNT)
        {
            brushCursor.SetActive(false);
            saving = true;
            Invoke("SaveTexture", 0.1f);
        }
    }

    void UpdateBrushCursor()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition) && !saving)
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
        }
        else
        {
            brushCursor.SetActive(false);
        }
    }

    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SaveTexture()
    {
        brushCounter = 0;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex;
        brushPool.ReturnAllBrushes();

        StartCoroutine(TextureSaver.SaveTextureToFile(tex, "CanvasTexture.png"));
        Invoke("ShowCursor", 0.1f);
    }

    public void LoadTextureFromFile()
    {
        string filePath = Path.Combine(Application.dataPath, "CanvasTexture.png");
        
        Texture2D loadedTexture = TextureSaver.LoadTextureFromFile(filePath);
        if (loadedTexture != null)
        {
            baseMaterial.mainTexture = loadedTexture;
        }
    }

    public void SetBrushSize(float newBrushSize)
    {
        brushSize = newBrushSize;
        brushCursor.transform.localScale = Vector3.one * brushSize;
    }

    public void SetBrushIntensity(float intensity)
    {
        brushIntensity = intensity;
        if (brushContainer != null)
        {
            foreach (Transform child in brushContainer.transform)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color originalColor = spriteRenderer.color;
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, brushIntensity);
                }
            }
        }
    }

    public void SetBrush(string brushName)
    {
        currentBrush = "TexturePainter-Instances/" + brushName;
        
        if (brushPool != null)
        {
            GameObject brushPrefab = Resources.Load<GameObject>(currentBrush);
            brushPool.UpdateBrushPrefab(brushPrefab);
        }
    }

    void ShowCursor()
    {
        saving = false;
    }
}
