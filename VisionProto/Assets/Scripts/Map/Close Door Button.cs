using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloseDoorButton : MonoBehaviour
{
    public GameObject enemyDeleteArea;

    public GameObject leftdoorAType;
    public GameObject rightdoorAType;
    public GameObject button;
    public GameObject mainBar;
    public bool isControl;

    [SerializeField]
    private float pressSpeed;

    private float deltaTime;

    // ������ ������ ��������.
    // Occlusion Culling �ʿ� ������ ������ SetActive False True�� �ϸ� ���������� ������?
    // �������� �̰� ������ �غ��� �� �� ����.

    private Vector3 pressedButton;
    private readonly Vector3 InitPosition = new Vector3(0, 6.17299986f, 0.303000003f);

    private int wallLayer;

    void Start()
    {
        // Start �κп��� Door A Type Component�� �޾ƿ´�.
        // �ű⼭ isClose��� ������ Ȱ��ȭ�ؼ� �ݰ� �Ѵ�.

        pressedButton = button.transform.localPosition + new Vector3(0, -0.15f, 0);
        deltaTime = 0f;

        int powLayer = LayerMask.GetMask("Wall");
        wallLayer = (int)Mathf.Ceil(Mathf.Log(powLayer) / Mathf.Log(2));
    }

    void Update()
    {
        if (isControl)
        {
            // Button
            button.transform.localPosition = Vector3.Slerp(button.transform.localPosition, pressedButton, Time.deltaTime * pressSpeed);
            deltaTime += Time.deltaTime;

            if (deltaTime > pressSpeed)
            {
                button.transform.localPosition = InitPosition;
                deltaTime = 0f;
                isControl = false;

                // Control ���� �� ��ư Object�� �������� �ٽ� ����ġ�� ���ƿ��� ���� ������.
                OpenDoorAType leftOpenDoor = leftdoorAType.GetComponent<OpenDoorAType>();
                OpenDoorAType rightOpenDoor = rightdoorAType.GetComponent<OpenDoorAType>();

                if (leftOpenDoor != null && rightOpenDoor != null)
                {
                    leftOpenDoor.leftDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    leftOpenDoor.rightDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;

                    button.GetComponent<InteractionObject>().isDone = true;
                    mainBar.GetComponent<InteractionObject>().isDone = true;

                    leftOpenDoor.isClose = true;
                    rightOpenDoor.isClose = true;
                    leftOpenDoor.deltaTime = 0f;
                    rightOpenDoor.deltaTime = 0f;

                    mainBar.tag = "Wall";
                    mainBar.layer = wallLayer;
                    button.tag = "Wall";
                    button.layer = wallLayer;
                }

                if(enemyDeleteArea != null)
                {
                    TwowaySwitch twowayButton = enemyDeleteArea.GetComponent<TwowaySwitch>();

                    if(twowayButton != null) 
                    {
                        twowayButton.isClose = true;
                    }
                }
            }
        }
    }
}
