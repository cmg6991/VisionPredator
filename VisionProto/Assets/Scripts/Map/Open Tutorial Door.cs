using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class OpenTutorialDoor : MonoBehaviour, IListener
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject tutorialDecal;

    // �� ���� Open Door
    private Vector3 openLeftDoor;
    private Vector3 openRightDoor;

    private readonly Vector3 leftDoorInitPosition = new Vector3(-1.33287919f, 0, -0.259704709f);
    private readonly Vector3 rightDoorInitPosition = new Vector3(1.34399998f, 0, -0.254000008f);

    private bool isOpen;
    public float deltaTime;
    public float count;
    private TutorialStage currentStage;

    // Enum���� �����ϱ�.
    public TutorialStage tutorialStage;

    private bool isClose;

    private bool isClear;

    // Start is called before the first frame update
    void Start()
    {
        /// ���ʰ� ������ ���� �ϳ� �����ؼ� ���� ȸ���� ������ �����´�.
        Vector3 doorRotation = leftDoor.transform.rotation.eulerAngles;
        float theta = doorRotation.y;

        /// Mathf.Sin, Math.Cos�� ����ϱ� ���� Radian�� ����ؾ� �ؼ� Degree -> Radian���� �ٲ۴�.
        float radian = theta * Mathf.Deg2Rad;

        /// Door�� Position�� �����صд�.
        Vector3 openDoorPosition = new Vector3(3f, 0, 0);
        openLeftDoor = leftDoor.transform.localPosition - openDoorPosition;
        openRightDoor = rightDoor.transform.localPosition + openDoorPosition;

        EventManager.Instance.AddEvent(EventType.TutorialOpenDoor, OnEvent);
    }

    /// <summary>
    /// ���� Stage�� �ִ´�.
    /// </summary>
    /// <param name="inputStage">���� Tutorial Stage</param>
    public void OpenDoor(TutorialStage inputStage)
    {
        currentStage = inputStage;
        isOpen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClose)
        {
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, leftDoorInitPosition, Time.deltaTime * count);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, rightDoorInitPosition, Time.deltaTime * count);
            deltaTime += Time.deltaTime;

            if (deltaTime > count)
            {
                deltaTime = 0f;
                leftDoor.transform.localPosition = leftDoorInitPosition;
                rightDoor.transform.localPosition = rightDoorInitPosition;
            }
            return;
        }

        if (currentStage != tutorialStage)
        {
            // ���� State�� Stage�� ������ isOpen�� Ȱ��ȭ�� �Ǿ��ٸ�
            return;
        }

        if (isOpen)
        {
            if (deltaTime == 0)
            {
                SoundManager.Instance.PlayEffectSound(SFX.OpenDoor_Big, this.transform.parent);
                tutorialDecal.SetActive(true);
            }
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, openLeftDoor, Time.deltaTime * count);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, openRightDoor, Time.deltaTime * count);
            deltaTime += Time.deltaTime;
        }

        // �ʱ� Transform�� ���� Transform�� ����ؼ� ��ȭ����ŭ ���� �ߴٸ� ��
        if (deltaTime > (count * 0.5f))
        {
            deltaTime = 0f;
            isOpen = false;
            isClear = true;

            leftDoor.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rightDoor.gameObject.GetComponent<MeshRenderer>().enabled = false;
            leftDoor.gameObject.GetComponent<MeshCollider>().enabled = false;
            rightDoor.gameObject.GetComponent<MeshCollider>().enabled = false;
            EventManager.Instance.RemoveEvent(EventType.TutorialOpenDoor, OnEvent);
        }
    }

    public void CloseDoor()
    {
        isClose = true;

        tutorialDecal.SetActive(false);
        leftDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;
        rightDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;
        leftDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
        rightDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch(eventType) 
        {
            case EventType.TutorialOpenDoor:
                {
                    TutorialDoorState doorState = (TutorialDoorState)param;
                    currentStage = doorState.stage;
                    isOpen = doorState.isOpen;
                }
                break;
        }
    }
}

public struct TutorialDoorState
{
    public TutorialStage stage;
    public bool isOpen;
}
