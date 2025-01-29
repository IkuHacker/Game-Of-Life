using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public Camera cameraMain; // R�f�rence � la cam�ra � contr�ler
    public float zoomSpeed = 5f; // Vitesse du zoom
    public float minZoom = 5f; // Zoom minimum
    public float maxZoom = 20f; // Zoom maximum

    private float currentZoom;

    private void Start()
    {
        if (cameraMain == null)
        {
            cameraMain = Camera.main; // Associe la cam�ra principale si aucune n'est d�finie
        }

        currentZoom = cameraMain.orthographicSize; // Initialise le zoom � la valeur actuelle
    }

    public void OnZoom(InputValue value)
    {
        float scrollValue = value.Get<float>(); // R�cup�re la valeur de la molette
        currentZoom -= scrollValue * zoomSpeed; // Modifie le zoom en fonction de la molette

        // Applique les limites de zoom
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        cameraMain.orthographicSize = currentZoom; // Applique le zoom � la cam�ra
    }
}
