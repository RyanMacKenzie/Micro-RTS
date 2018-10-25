using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // Use this for initialization
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode zoomIn = KeyCode.E;
    public KeyCode zoomOut = KeyCode.Q;


    bool isSelecting = false;
    Vector3 mousePosition1;

    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }
    
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

   
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            GetViewportBounds(camera, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            Rect rect = GetScreenRect(mousePosition1, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(moveLeft))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - gameObject.GetComponent<Camera>().orthographicSize * (Time.deltaTime), gameObject.transform.position.y, gameObject.transform.position.z);
        if (Input.GetKey(moveRight))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + gameObject.GetComponent<Camera>().orthographicSize * (Time.deltaTime), gameObject.transform.position.y, gameObject.transform.position.z);
        if (Input.GetKey(moveUp))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.GetComponent<Camera>().orthographicSize * Time.deltaTime), gameObject.transform.position.z);
        if (Input.GetKey(moveDown))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - (gameObject.GetComponent<Camera>().orthographicSize * Time.deltaTime), gameObject.transform.position.z);
        if (Input.GetKey(zoomIn))
        {
            if (gameObject.GetComponent<Camera>().orthographicSize > 5)
                gameObject.GetComponent<Camera>().orthographicSize -= 1;
        }
        if (Input.GetKey(zoomOut))
        {
            if (gameObject.GetComponent<Camera>().orthographicSize < 50)
                gameObject.GetComponent<Camera>().orthographicSize += 1;
        }
       
             gameObject.GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 10;
        if (gameObject.GetComponent<Camera>().orthographicSize < 5)
            gameObject.GetComponent<Camera>().orthographicSize = 5;
        if (gameObject.GetComponent<Camera>().orthographicSize > 50)
            gameObject.GetComponent<Camera>().orthographicSize = 50;

        // If we press the left mouse button, begin selection and remember the location of the mouse
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;

            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>())
            {
                if (selectableObject.selectionCircle != null)
                {
                    Destroy(selectableObject.selectionCircle.gameObject);
                    selectableObject.selectionCircle = null;
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            var selectedObjects = new List<GameObject>();
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject))
                {
                    selectedObjects.Add(selectableObject.gameObject);
                }
            }
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().localPlayer.GetComponent<PlayerScript>().SelectedUnits = selectedObjects;
            isSelecting = false;
        }

        // Highlight all objects within the selection box
    }
}
