using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum RobotState
{
    Following, // the player
    Moving, // towards target
    Standing // at target
}

public class RobotScript : MonoBehaviour
{
    [Header("Robot Components")]
    [SerializeField] SpriteRenderer RobotSprRend;
    [SerializeField] Animation Animation;
    [SerializeField] NavMeshAgent NavAgent;
    
    [Header("Player Components")]
    [SerializeField] RobotController Controller;
    [SerializeField] Transform PlrTransform;
    [SerializeField] SpriteRenderer PlrSprRend;
    [SerializeField] GameObject PlrRing;

    [Header("Character Ring")]
    [SerializeField] GameObject CharRing;
    [SerializeField] SpriteRenderer CharRingSprRend;
    [SerializeField] Color OnColor;
    [SerializeField] Color OffColor;

    [Header("Command Ring")]
    [SerializeField] GameObject CmdRing;
    [SerializeField] SpriteRenderer CmdRingSprRend;
    [SerializeField] Color CmdColor;
    [SerializeField] Color CmdUnavailableColor;

    [Header("Settings")] // i.e. Constants
    [SerializeField] [Min(0)] float CmdCooldown = 0.1f; // seconds
    float currentFollowDistance;

    //==== VARIABLES ====\\
    
    bool isCommanding = false;
    int placeInOrder;
    // Robot state
    public RobotState state = RobotState.Following;
    // Feet-related positions
    Vector2 robotPos;
    Vector2 plrPos;
    Vector2 plrDist;
    // Rings
    bool isCharRingOn = false;
    // Moving to commanded position
    float startTime = 0f;
    Vector2 targetPos;

    //==== PLAYER FOLLOWING ====\\

    Vector2 GetClosestPointInPlayerRange()
    {
        float r0 = plrDist.magnitude;
        float r = currentFollowDistance;

        if (r0 <= r || Mathf.Approximately(r0, r)) return transform.position;

        float x0 = plrDist.x;
        float y0 = plrDist.y;

        float x = r * (x0 / r0); // = r * cos(angle)
        float y = r * (y0 / r0); // = r * sin(angle)

        return plrPos + new Vector2(x, y);
    }

    Vector2 ReverseVector(Vector2 vector) {
        return vector * new Vector2(-1, -1);
    }

    void SetPosition()
    {
        switch(state)
        {
            case(RobotState.Following):
                // destination is set every frame (here) if it's different from the current one
                if (Vector2.Distance(transform.position, plrPos) <= currentFollowDistance) break;
                
                Vector2 closestPoint = GetClosestPointInPlayerRange();

                if (targetPos == closestPoint) break;

                targetPos = closestPoint;
                NavAgent.SetDestination(targetPos);
                break;

            case(RobotState.Moving):
                // destination is set once (not here)
                if (Vector2.Distance(transform.position, targetPos) > 0.05f) break;

                state = RobotState.Standing;
                break;
        }
    }

    void SetRotation()
    {
        float x = state == RobotState.Moving ? ReverseVector(NavAgent.desiredVelocity).x : plrDist.x;

        // if the target is to the robot's right (or in it) and the robot is turned to the left
        if (x <= 0 && transform.rotation.eulerAngles.y == 180)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        // else if the target is to the robot's left and the robot is turned to the right
        else if (x > 0 && transform.rotation.eulerAngles.y == 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void SetAnimationAndSortOrder()
    {
        string animationName = "ride_";
        Vector2 distance = state == RobotState.Moving ? ReverseVector(NavAgent.desiredVelocity) : plrDist;
        animationName += distance.y < 0f ? "back" : "front"; // y < 0 -> player is on robot's back
        Animation.Play(animationName);

        RobotSprRend.sortingOrder = PlrSprRend.sortingOrder + (plrDist.y < 0f ? 1 : -1);
    }

    //==== SELECTION (via mouse) ====\\

    // Ring manipulation
    void SetCharRingOn(bool on)
    {
        isCharRingOn = on;
        CharRingSprRend.color = on ? OnColor : OffColor;
        CharRing.SetActive(on);
    }

    void SetCmdAvailability(bool on)
        => CmdRingSprRend.color = on ? CmdColor : CmdUnavailableColor;

    void SetRingsOn(bool on)
    {
        SetCharRingOn(on);
        CmdRing.SetActive(on);
        isCommanding = on;

        if (!on) PlrRing.SetActive(false);
    }

    void MoveCmdRingToCursor()
    {
        if (!isCommanding) return;

        CmdRing.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Handling mouse clicks
    public void OnMouseUp() // fires when left-clicked on the robot
    {
        SetRingsOn(!isCommanding);

        if (isCommanding)
            startTime = Time.time;
    }
    
    void OnMouseLeftClick() // fires when left-clicked anywhere
    {
        if (!isCommanding || !Input.GetMouseButtonUp(0) || (Time.time - startTime < CmdCooldown)) return;
        
        if (CmdRing.activeSelf) // if commanding to move to target
        {
            if (CmdRingSprRend.color == CmdUnavailableColor) return;

            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            NavAgent.SetDestination(targetPos);

            Controller.StopFollowing(this);
            state = RobotState.Moving;
        }
        else if (PlrRing.activeSelf) // if commanding to follow player
        {
            Controller.StartFollowing(this);
            state = RobotState.Following;
        }

        SetRingsOn(false);
    }

    void OnMouseRightClick() // fires when right-clicked anywhere
    {
        if (Input.GetMouseButtonUp(1)) SetRingsOn(false);
    }

    // Handling mouse hovering
    void OnMouseEnter() {
        if (!isCharRingOn) CharRing.SetActive(true);
    }
    void OnMouseExit() {
        if (!isCharRingOn) CharRing.SetActive(false);
    }

    Transform GetHoveredTransform()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (!hit) return null;
        return hit.transform;
    }

    void OnMouseHoverChange()
    {
        if (!isCommanding) return;

        Transform hoveredTransform = GetHoveredTransform();

        bool isHoveringOverTransform = hoveredTransform != null;
        SetCmdAvailability(isHoveringOverTransform && hoveredTransform.tag != "Impassable Tile");
        CmdRing.SetActive(true);
        PlrRing.SetActive(false);

        if (!isHoveringOverTransform) return;

        bool isHoveringOverPlayer = hoveredTransform == PlrTransform;
        CmdRing.SetActive(!isHoveringOverPlayer);
        PlrRing.SetActive(isHoveringOverPlayer);
    }

    //========\\

    void Start()
    {
        Controller.StartFollowing(this);

        NavAgent.updateRotation = false;
        NavAgent.updateUpAxis = false;
    }

    void Update()
    {
        // Updating values
        robotPos = transform.position;
        plrPos = PlrTransform.position;
        plrDist = robotPos - plrPos;
        currentFollowDistance = Controller.FollowDistance * Controller.GetPlaceInOrder(this);

        // Updating the character based on player's movement
        SetPosition();
        SetRotation();
        SetAnimationAndSortOrder();

        // Updating rings based on player's input
        OnMouseLeftClick();
        OnMouseRightClick();
        OnMouseHoverChange();
        MoveCmdRingToCursor();
    }
}
