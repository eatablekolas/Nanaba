using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Components
    Transform PlayerTransform;
    Animation PlayerAnimation;

    // Variables
    [SerializeField] float playerSpeed = 0.02f;
    Vector2 inputAxis = new Vector2(0f, 0f);
    Vector2 inputAxisAbs = new Vector2(0f, 0f);
    string previousSide = "front";

    // Constants
    float DIAGONAL_MODIFIER = 2 / Mathf.Sqrt(5);

    void Awake()
    {
        PlayerTransform = transform;
        PlayerAnimation = gameObject.GetComponent<Animation>();
    }

    void UpdateAxis()
    {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputAxisAbs = new Vector2(Mathf.Abs(inputAxis.x), Mathf.Abs(inputAxis.y));
    }

    void RotateIfPlayerIsTurning()
    {
        if (inputAxis.x == 1 && PlayerTransform.rotation.y == 1)
        {
            PlayerTransform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (inputAxis.x == -1 && PlayerTransform.rotation.y == 0)
        {
            PlayerTransform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    void ApplyAnimation()
    {
        string animationName;

        if (inputAxisAbs.x == 1 || inputAxisAbs.y == 1)
        {
            animationName = "walk_";
        }
        else
        {
            animationName = "idle_";
        }

        if (inputAxis.y == 1)
        {
            animationName += "back";
            previousSide = "back";
        }
        else if (inputAxis.y == -1)
        {
            animationName += "front";
            previousSide = "front";
        }
        else
        {
            animationName += previousSide;
        }

        PlayerAnimation.Play(animationName);
    }

    Vector2 CalculateDiagonalModifier()
    {
        Vector2 diagonalModifier = new Vector2(1f, 1f);

        if (inputAxisAbs.x == 1 && inputAxisAbs.y == 1) // if the player goes diagonally
        {
            diagonalModifier.x = DIAGONAL_MODIFIER;
            diagonalModifier.y = DIAGONAL_MODIFIER / 2;
        }

        return diagonalModifier;
    }

    Vector3 CalculateDistance()
    {
        Vector2 diagonalModifier = CalculateDiagonalModifier();

        float xDistance = inputAxis.x * playerSpeed * diagonalModifier.x;
        float yDistance = inputAxis.y * playerSpeed * diagonalModifier.y;

        return new Vector3(xDistance, yDistance, 0);
    }

    void Update()
    {
        UpdateAxis();
        RotateIfPlayerIsTurning();
        ApplyAnimation();
        PlayerTransform.position = PlayerTransform.position + CalculateDistance();
    }
}
