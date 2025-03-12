using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class OpenDoorAType : MonoBehaviour, IListener
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject outLineDoor;

    public GameObject Text;

    // �� ���� Open Door
    private Vector3 openLeftDoor;  
    private Vector3 openRightDoor;

    // ȸ���Ǿ� �ִ� ���� ���� ����� ã�ƺ���.
    private bool isOpen;
    public float deltaTime;

    public float count;
    public bool isClose;

    private bool isdone;
    public bool isOpenDoor;

    // Enemy���� ��Ƴ���.
    public List<GameObject> enemys;

    // ���� ���� �ν��Ѵ�.
    public GameObject nextLeftDoor;
    public GameObject nextRightDoor;

    // �� Bool ������ True�� Enemy List�� ���Ƽ� ���� �ֵ��� �ִ��� �˻��ϴ� ������. false�� �ƹ��͵� ����. �Լ��� ���� �ʴ´�.
    public bool isEnemyContact;

    // �� Bool ������ ����ý����� ���ٸ� ���� �������� ������ ������ �ʴ´�.
    public bool isNextDoorOpen;

    // �� ���� ���� ���� Door��� �ٽ� ����.
    public bool isBossDoor;

    // Clear Scene Door ����
    public bool isClearDoor;

    // Test�뵵
    public bool isTest;

    /// <summary>
    /// ������ �߰��� ���� �ǽð����� �ݿ��ϱ� ����
    /// </summary>
    private NavMeshObstacle obstacle;
    private NavMeshObstacle doorObstacle;

    private readonly Vector3 leftDoorInitPosition = new Vector3(-1.33287919f, 0, -0.259704709f);
    private readonly Vector3 rightDoorInitPosition = new Vector3(1.34399998f, 0, -0.254000008f);



    // Start is called before the first frame update
    void Start()
    {
        // Door�� ���� Rotation�� �޴´�. 
        // Rotation�� ���� ���� ������ Position�� �ٲ�� �ϱ� ���� �ʿ��� �غ� �۾�
        deltaTime = 0;
        /// ���ʰ� ������ ���� �ϳ� �����ؼ� ���� ȸ���� ������ �����´�.
        Vector3 doorRotation = leftDoor.transform.rotation.eulerAngles;
        float theta = doorRotation.y;

        /// Mathf.Sin, Math.Cos�� ����ϱ� ���� Radian�� ����ؾ� �ؼ� Degree -> Radian���� �ٲ۴�.
        float radian = theta * Mathf.Deg2Rad;

        /// Door�� Position�� �����صд�.
        Vector3 openDoorPosition = new Vector3(3f, 0, 0);
        openLeftDoor = leftDoor.transform.localPosition - openDoorPosition;
        openRightDoor = rightDoor.transform.localPosition + openDoorPosition;

        if (isTest)
        {
            EventManager.Instance.AddEvent(EventType.NPCDeath, OnEvent);
        }

        obstacle = GetComponent<NavMeshObstacle>();

        if (outLineDoor != null)
        {
            doorObstacle = outLineDoor.GetComponent<NavMeshObstacle>();
        }

        // enemsy�� �� ��� �ְ�, isNextOpenDoor�� False�� AddEvent�� �Ѵ�.


    }

    // Update is called once per frame
    void Update()
    {
        if (isdone)
            return;

        if (isClose)
        {
            if (deltaTime == 0)
            {
                SoundManager.Instance.PlayEffectSound(SFX.OpenDoor_Big, this.transform.parent);
                leftDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
                rightDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
                obstacle.enabled = true;
                doorObstacle.enabled = true;
            }

            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, leftDoorInitPosition, Time.deltaTime * count);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, rightDoorInitPosition, Time.deltaTime * count);
            deltaTime += Time.deltaTime;

            if (deltaTime > count)
            {
                deltaTime = 0f;
                leftDoor.transform.localPosition = leftDoorInitPosition;
                rightDoor.transform.localPosition = rightDoorInitPosition;
                isdone = true;
            }
            return;
        }

        if (isOpen)
        {
            if (deltaTime == 0)
                SoundManager.Instance.PlayEffectSound(SFX.OpenDoor_Big, this.transform.parent);
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, openLeftDoor, Time.deltaTime * count);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, openRightDoor, Time.deltaTime * count);
            deltaTime += Time.deltaTime;
            obstacle.enabled = false;
            doorObstacle.enabled = false;
        }

        // �ʱ� Transform�� ���� Transform�� ����ؼ� ��ȭ����ŭ ���� �ߴٸ� ��
        if (deltaTime > (count * 0.5f))
        {
            deltaTime = 0f;
            isOpen = false;
            isOpenDoor = true;
            leftDoor.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rightDoor.gameObject.GetComponent<MeshRenderer>().enabled = false;
            leftDoor.gameObject.GetComponent<MeshCollider>().enabled = false;
            rightDoor.gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

    public void DoorOpen()
    {
        if (isOpenDoor)
            return;

        if(isClearDoor)
        {
            // Virtual Camera�� ã�� 
            GameObject camera = GameObject.Find("Virtual Camera");
            EventManager.Instance.RemoveEvent(EventType.HitBulletRotation, OnEvent);

            if (camera == null)
                Debug.Log("None Virtual Camera");
            else
            {
                VPRenderFeature renderFeature;
                camera.transform.parent.TryGetComponent<VPRenderFeature>(out renderFeature);
                // �̰Ÿ� �ص� Time Scale�� 0�̿��� ������ �ȵȴ�. �׷��� �ױ� ������ ����
                renderFeature.isGameClear = true;
                renderFeature.FadeInFadeOut();
            }
            return;
        }

        isOpen = true;
        // Next Door �ȿ��� isEnemyContact�� True�� �Ѵ�.
        if (nextLeftDoor != null && nextRightDoor != null)
        {
            OpenDoorAType openDoorAType = nextLeftDoor.GetComponent<OpenDoorAType>();
            openDoorAType.isEnemyContact = true;

            openDoorAType = nextRightDoor.GetComponent<OpenDoorAType>();
            openDoorAType.isEnemyContact = true;
        }
    }

    public void DoorClose()
    {
        // ���⼭ ���� �������
        isClose = true;
        leftDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;
        rightDoor.gameObject.GetComponent<MeshRenderer>().enabled = true;
        leftDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
        rightDoor.gameObject.GetComponent<MeshCollider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isClose && isNextDoorOpen)
        {
            DoorOpen();
        }
    }

    // NPC�� ������ Loop �Ѵ�.
    private void NPCLoop(GameObject enemy)
    {
        // NPC Loop�� �� ����ٸ� ������! ������� �ʴٸ�? ��� �˻��ϸ鼭 ���� �ֵ��� �˻��Ѵ�.
        if (!isEnemyContact)
            return;

        // List�� ���Ƽ� �ش��ϴ� Enemy�� ������ üũ�Ѵ�. ������ ���� �ƴϸ� ���� �� �����Ǽ� ��� �ֵ��� ������.

        GameObject enemyObject = null;

        foreach (GameObject obj in enemys)
        {
            if (obj == enemy)
                enemyObject = obj;
        }

        if (enemyObject != null)
            enemys.Remove(enemyObject);

        if (!enemys.Any())
        {
            isNextDoorOpen = true;

            ShowText();
            EventManager.Instance.RemoveEvent(EventType.NPCDeath, OnEvent);
            DoorOpen();
        }
    }

    void ShowText()
    {
        Text.SetActive(true);
        StartCoroutine(DelayFalse(3f));
    }

    IEnumerator DelayFalse(float delay)
    {
        yield return new WaitForSeconds(delay);
        Text.SetActive(false);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.NPCDeath:

                GameObject enemy = (GameObject)param;

                NPCLoop(enemy);
                break;
        }
    }

}