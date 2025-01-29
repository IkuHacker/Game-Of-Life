using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Vitesse de déplacement
    private Vector2 moveInput; // Stocke l'entrée utilisateur pour le mouvement

    // Fonction appelée par le système d'Input
    // Cette méthode est appelée automatiquement par le système d'Input
    public void OnMove(InputValue value)
    {
        // Récupère la valeur des axes de mouvement (Vector2)
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Déplace la caméra en fonction de l'entrée utilisateur
        Vector2 movement = new Vector2(moveInput.x, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(movement.x, movement.y, 0);
    }
}
