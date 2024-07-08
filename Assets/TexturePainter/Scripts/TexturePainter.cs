﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;


public class TexturePainter : MonoBehaviour
{
    public GameObject brushCursor, brushContainer; //The cursor that overlaps the model and our container for the brushes painted
    public Camera sceneCamera, canvasCam;  
    public Sprite cursorPaint; 
    public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
    public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)


    float brushSize = 1.0f; //The size of our brush
    float brushIntensity = 1.0f;
    Color brushColor; //The selected color
    int brushCounter = 0, MAX_BRUSH_COUNT = 1000; //To avoid having millions of brushes
    bool saving = false; //Flag to check if we are saving the texture
    private string currentBrush = "TexturePainter-Instances/BrushEntity";

    void Update()
    {
        brushColor = ColorSelector.GetColor();  //Updates our painted color with the selected color
        if (Input.GetMouseButton(0))
        {
            DoAction();
        }
        UpdateBrushCursor();
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction()
    {
        if (saving)
            return;
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            GameObject brushObj;


            brushObj = (GameObject)Instantiate(Resources.Load(currentBrush)); //Paint a brush
            brushObj.GetComponent<SpriteRenderer>().color = brushColor; //Set the brush color

            brushColor.a = brushSize * 2.0f; // Brushes have alpha to have a merging effect when painted over.
            brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
            brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
            brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush
        }
        brushCounter++; //Add to the max brushes
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
    //Returns the position on the texuremap according to a hit in the mesh collider
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
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex; //Put the painted texture as the base
        foreach (Transform child in brushContainer.transform)
        {//Clear brushes
            Destroy(child.gameObject);
        }

        // Save the texture to file
        StartCoroutine(SaveTextureToFile(tex));

        Invoke("ShowCursor", 0.1f);
    }

    public void LoadTextureFromFile()
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, "CanvasTexture.png");

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2); 

            if (texture.LoadImage(fileData)) 
            {
                baseMaterial.mainTexture = texture; 
                Debug.Log("Loaded texture from file: " + filePath);
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + filePath);
            }
        }
        else
        {
            Debug.LogError("Texture file not found: " + filePath);
        }
    }

    //Show again the user cursor (To avoid saving it to the texture)
    void ShowCursor()
    {
        saving = false;
    }

    public void SetBrushSize(float newBrushSize)
    { //Sets the size of the cursor brush or decal
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
    }

    IEnumerator SaveTextureToFile(Texture2D savedTexture)
    {
        string fullPath = System.IO.Path.Combine(Application.dataPath, "CanvasTexture.png");
        var bytes = savedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

        Debug.Log("<color=orange>Saved Successfully!</color> " + fullPath);
        yield return null;
    }


}
