using UnityEngine;

public class NPCFaceController : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Rendering")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite; // right-facing base

    [Header("Rotation")]
    public float rotationSpeed = 8f;
    public bool smoothRotation = true;

    [Header("Angle Thresholds")]
    public float frontThreshold = 45f;
    public float backThreshold = 135f;

    private Vector3 baseForward;

    private void Start()
    {
        // store ORIGINAL facing direction
        baseForward = transform.forward;
    }

    private void Update()
    {
        if (player == null) return;

        FacePlayer();
        UpdateSprite();
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (smoothRotation)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    void UpdateSprite()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0f;

        if (dirToPlayer.sqrMagnitude < 0.001f) return;

        
        float angle = Vector3.SignedAngle(baseForward, dirToPlayer, Vector3.up);
        angle = -angle;
        float absAngle = Mathf.Abs(angle);

        // FRONT
        if (absAngle <= frontThreshold)
        {
            spriteRenderer.sprite = frontSprite;
            spriteRenderer.flipX = false;
        }
        // BACK
        else if (absAngle >= backThreshold)
        {
            spriteRenderer.sprite = backSprite;
            spriteRenderer.flipX = false;
        }
        // SIDE
        else
        {
            spriteRenderer.sprite = sideSprite;
            spriteRenderer.flipX = angle < 0f;
        }
    }
}