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

    // 이 문은 Open Door
    private Vector3 openLeftDoor;  
    private Vector3 openRightDoor;

    // 회전되어 있는 문을 여는 방법을 찾아보자.
    private bool isOpen;
    public float deltaTime;

    public float count;
    public bool isClose;

    private bool isdone;
    public bool isOpenDoor;

    // Enemy들을 담아놨다.
    public List<GameObject> enemys;

    // 다음 문을 인식한다.
    public GameObject nextLeftDoor;
    public GameObject nextRightDoor;

    // 이 Bool 변수가 True면 Enemy List를 돌아서 죽은 애들이 있는지 검사하는 변수다. false면 아무것도 안함. 함수를 돌지 않는다.
    public bool isEnemyContact;

    // 이 Bool 변수는 개폐시스템이 없다면 문이 열리지만 없으면 열리지 않는다.
    public bool isNextDoorOpen;

    // 이 문이 보스 전용 Door라면 다시 닫자.
    public bool isBossDoor;

    // Clear Scene Door 전용
    public bool isClearDoor;

    // Test용도
    public bool isTest;

    /// <summary>
    /// 리나가 추가함 길을 실시간으로 반영하기 위해
    /// </summary>
    private NavMeshObstacle obstacle;
    private NavMeshObstacle doorObstacle;

    private readonly Vector3 leftDoorInitPosition = new Vector3(-1.33287919f, 0, -0.259704709f);
    private readonly Vector3 rightDoorInitPosition = new Vector3(1.34399998f, 0, -0.254000008f);



    // Start is called before the first frame update
    void Start()
    {
        // Door의 현재 Rotation을 받는다. 
        // Rotation에 따라 문이 열리는 Position이 바뀌게 하기 위해 필요한 준비 작업
        deltaTime = 0;
        /// 왼쪽과 오른쪽 문중 하나 선택해서 현재 회전한 각도를 가져온다.
        Vector3 doorRotation = leftDoor.transform.rotation.eulerAngles;
        float theta = doorRotation.y;

        /// Mathf.Sin, Math.Cos을 사용하기 위해 Radian을 사용해야 해서 Degree -> Radian으로 바꾼다.
        float radian = theta * Mathf.Deg2Rad;

        /// Door의 Position을 저장해둔다.
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

        // enemsy가 안 비어 있고, isNextOpenDoor가 False면 AddEvent를 한다.


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

        // 초기 Transform과 나중 Transform을 계산해서 변화량만큼 도달 했다면 끝
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
            // Virtual Camera를 찾고 
            GameObject camera = GameObject.Find("Virtual Camera");
            EventManager.Instance.RemoveEvent(EventType.HitBulletRotation, OnEvent);

            if (camera == null)
                Debug.Log("None Virtual Camera");
            else
            {
                VPRenderFeature renderFeature;
                camera.transform.parent.TryGetComponent<VPRenderFeature>(out renderFeature);
                // 이거를 해도 Time Scale이 0이여서 실행이 안된다. 그래서 죽기 전까지 가능
                renderFeature.isGameClear = true;
                renderFeature.FadeInFadeOut();
            }
            return;
        }

        isOpen = true;
        // Next Door 안에서 isEnemyContact를 True로 한다.
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
        // 여기서 게임 종료시켜
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

    // NPC가 죽으면 Loop 한다.
    private void NPCLoop(GameObject enemy)
    {
        // NPC Loop가 다 비었다면 열린다! 비어있지 않다면? 계속 검사하면서 죽은 애들을 검사한다.
        if (!isEnemyContact)
            return;

        // List를 돌아서 해당하는 Enemy랑 같은지 체크한다. 맞으면 삭제 아니면 진행 다 삭제되서 비어 있따면 열린다.

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