using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Vitesse de d�placement
    private Vector2 moveInput; // Stocke l'entr�e utilisateur pour le mouvement

    // Fonction appel�e par le syst�me d'Input
    // Cette m�thode est appel�e automatiquement par le syst�me d'Input
    public void OnMove(InputValue value)
    {
        // R�cup�re la valeur des axes de mouvement (Vector2)
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // D�place la cam�ra en fonction de l'entr�e utilisateur
        Vector2 movement = new Vector2(moveInput.x, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(movement.x, movement.y, 0);
    }
}
