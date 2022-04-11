using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] [Min(0)] public float FollowDistance = 2f;
    public List<RobotScript> controlledRobots = new List<RobotScript>();
    List<RobotScript> followingRobots = new List<RobotScript>();

    public int GetPlaceInOrder(RobotScript requestingRobot)
    {
        return followingRobots.FindIndex(robot => robot == requestingRobot) + 1;
    }

    public void StartFollowing(RobotScript requestingRobot)
    {
        if (followingRobots.Find(robot => robot == requestingRobot)) return;

        followingRobots.Add(requestingRobot);
    }

    public void StopFollowing(RobotScript requestingRobot)
    {
        followingRobots.Remove(requestingRobot);
    }

    void Awake()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot"); 
        
        foreach (GameObject robot in robots)
        {
            controlledRobots.Add(robot.GetComponent<RobotScript>());
        }
    }

    void Update()
    {
        for (int i = 0; i < 10; i++) // 0-9
        {
            if (Input.GetKeyUp(i.ToString()) || Input.GetKeyUp("[" + i.ToString() + "]")) // [0] = Numpad 0
            {
                if (i > controlledRobots.Count) break;

                RobotScript selectedRobot = controlledRobots[i - 1];
                selectedRobot.OnMouseUp();

                break;
            }
        }
    }
}
