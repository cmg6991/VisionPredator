using UnityEngine;

public class OpenDoorBType : MonoBehaviour
{
    // Open, Close 상태를 받아와야 한다.
    Quaternion openDoorQuaternion;
    Quaternion closeDoorQuaternion;
    Quaternion reverseOpenDoorQuaternion;
    Quaternion reverseCloseDoorQuaternion;
    public bool isForwardOpen;
    public bool isReverseOpen;
    bool isForwardOpenDoor;
    bool isReverseOpenDoor;
    bool closeForwardDoor;
    bool closeReverseDoor;

    /// 문 여는 속도를 조절해주는 변수다.
    public float doorSpeed;

    // 이름을 뭘로 해야하지? 이 변수는 열고 닫을 수 있다.
    public bool isControl;

    private bool isPlaying;

    public GameObject nextLeftDoor;
    public GameObject nextRightDoor;

    // Start is called before the first frame update
    void Start()
    {
        isControl = false;
        doorSpeed = 2.5f;
        // 구 버전
        // 신 버전을 새로 만들어야 한다.
        // localRotation -90도 90도로 열려야 한다. 닫는건 localRotation이 0으로 되어야 한다. 
        openDoorQuaternion = Quaternion.Euler(0, -90f, 0);
        closeDoorQuaternion = Quaternion.Euler(0, 90, 0);
        reverseOpenDoorQuaternion = Quaternion.Euler(0, 90f, 0);
        reverseCloseDoorQuaternion = Quaternion.Euler(0, 0, 0);

    }

    private void Update()
    {
        if (isControl)
        {
            if (isForwardOpenDoor)
            {
                closeForwardDoor = true;
            }

            if (isReverseOpenDoor)
            {
                closeReverseDoor = true;
            }
        }

        if (closeForwardDoor)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, closeDoorQuaternion, Time.deltaTime * (doorSpeed + 0.25f));

            if (!isPlaying)
            {
                SoundManager.Instance.PlayEffectSound(SFX.Close_Door, this.transform);
                isPlaying = true;
            }

            if (transform.localRotation.eulerAngles.y <= closeDoorQuaternion.eulerAngles.y + 1f)
            {
                closeForwardDoor = false;
                isForwardOpenDoor = false;
                isControl = false;
                isPlaying = false;
            }
        }

        if (closeReverseDoor)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, reverseCloseDoorQuaternion, Time.deltaTime * (doorSpeed + 0.25f));

            if (!isPlaying)
            {
                SoundManager.Instance.PlayEffectSound(SFX.Close_Door, this.transform);
                isPlaying = true;
            }

            if (transform.localRotation.eulerAngles.y <= reverseCloseDoorQuaternion.eulerAngles.y + 1f)
            {
                closeReverseDoor = false;
                isReverseOpenDoor = false;
                isControl = false;
                isPlaying = false;
            }
        }

        if (isForwardOpen)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, openDoorQuaternion, Time.deltaTime * doorSpeed);

            if (transform.localRotation.eulerAngles.y <= openDoorQuaternion.eulerAngles.y + 1f)
            {
                isReverseOpen = false;
                isForwardOpen = false;
                isForwardOpenDoor = true;
            }
        }
        else if (isReverseOpen)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, reverseOpenDoorQuaternion, Time.deltaTime * doorSpeed);

            if (transform.localRotation.eulerAngles.y >= reverseOpenDoorQuaternion.eulerAngles.y - 1f)
            {
                isReverseOpen = false;
                isForwardOpen = false;
                isReverseOpenDoor = true;
            }
        }
    }

    public void DoorOpen(bool isReverse)
    {
        if ((!isReverseOpenDoor && !isForwardOpenDoor) && (!isReverseOpen && !isForwardOpen))
        {
            if (isReverse)
                isReverseOpen = true;
            else
                isForwardOpen = true;

            SoundManager.Instance.PlayEffectSound(SFX.Open_Door, this.transform);

            if (nextRightDoor != null && nextLeftDoor != null)
            {
                OpenDoorAType openDoorAType = nextLeftDoor.GetComponent<OpenDoorAType>();
                openDoorAType.isEnemyContact = true;
                openDoorAType = nextRightDoor.GetComponent<OpenDoorAType>();
                openDoorAType.isEnemyContact = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && (!isReverseOpenDoor && !isForwardOpenDoor) && (!isReverseOpen && !isForwardOpen))
        {
            // Camera의 시선 벡터
            Vector3 myForward = Camera.main.transform.forward;

            // 문 시선 벡터
            Vector3 doorForward = this.transform.forward;

            // 둘이 내적한다.
            float dotValue = Vector3.Dot(myForward.normalized, doorForward.normalized);

            if (dotValue > 0)
                DoorOpen(false);
            else
                DoorOpen(true);
        }
    }
}
