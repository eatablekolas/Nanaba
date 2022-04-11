using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMenu : MonoBehaviour
{
    // Robots
    [SerializeField] RobotController Controller;
    List<RobotScript> Robots;

    // Options
    [SerializeField] Transform Panel;
    Transform[] Options;
    [SerializeField] Image ExampleOptionImage;
    Image[] OptionImages;
    [SerializeField] GameObject ExampleOptionNameFrame;
    [SerializeField] Text ExampleOptionName;
    Text[] OptionNames;

    void Awake()
    {
        int length = Panel.childCount;
        Options = new Transform[length];
        OptionImages = new Image[length];
        OptionNames = new Text[length];

        foreach (Transform option in Panel)
        {
            int optionNumber;
            int.TryParse(option.name.Split()[1], out optionNumber);

            int i = optionNumber - 1;
            Options[i] = option;
            OptionImages[i] = option.Find(ExampleOptionImage.name).GetComponent<Image>();
            OptionNames[i] = option.Find(ExampleOptionNameFrame.name).Find(ExampleOptionName.name).GetComponent<Text>();
        }
    }

    void Start()
    {
        Robots = Controller.controlledRobots;

        for (int i = 0; i < Robots.Count && i < Options.Length; i++)
        {
            RobotScript robot = Robots[i];
            Transform option = Options[i];
            Image image = OptionImages[i]; 
            image.sprite = Resources.Load<Sprite>("Images/" + robot.name);
            image.color = robot.GetComponent<SpriteRenderer>().color;

            option.gameObject.SetActive(true);
        }
    }
}
