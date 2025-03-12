using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VPWeaponEffect : MonoBehaviour, IListener
{
    public GameObject leftHand;
    public GameObject rightHand;

    // Scratch Speed
    [SerializeField]
    private float speed = 2f;

    public bool isLeftAttack;
    public bool isRightAttack;
    private bool isVPState;
    private bool isTutorial;

    public GameObject leftEffect;
    public GameObject rightEffect;

    // �ʱ� ��
    public readonly Vector3 initPosition = new Vector3(0.3f, 0.3f, 0.3f);
    private readonly Vector3 endPosition = new Vector3(1.0f, 1.0f, 1.0f);

    private void Start()
    {
        // leftHand���� localTransform�� �����ؾ� �Ѵ�. -> ���� Ŀ����
        // �ȿ� �ڵ嵵 ���µ� ��� �ڽĿ��� VP State�� ��������? GetComponentInChild�� �ϸ� �ڱ� �ڽ� �����ؼ� �����´�. 

        // �θ𿡴� ���� �ڽĿ��� �ִ� �ŷ� �̿��ؼ� GameObject�� �ҷ�����.
        MeshRenderer leftHandEffectObject = leftHand.GetComponentInChildren<MeshRenderer>();
        MeshRenderer rightHandEffectObject = rightHand.GetComponentInChildren<MeshRenderer>();

        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
        EventManager.Instance.AddEvent(EventType.Tutorial, OnEvent);

        if (leftHandEffectObject != null)
            leftEffect = leftHandEffectObject.gameObject;
        else
            Debug.Log("None LeftHandObject");

        if (rightHandEffectObject != null)
            rightEffect = rightHandEffectObject.gameObject;
        else
            Debug.Log("None RightHandObject");
    }

    private void Update()
    {
        if (!isVPState || isTutorial)
            return;

        if (isLeftAttack)
        {
            leftHand.SetActive(true);
            rightHand.SetActive(false);

            /// leftEffect�� local position�� �ø���.
            float currentScale = Mathf.Lerp(leftEffect.transform.localScale.x, endPosition.x, Time.deltaTime * speed);
            leftEffect.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            float currentRoundScale = (float)System.Math.Round(currentScale, 2);
            float endRoundScale = (float)System.Math.Round(endPosition.x, 2);

            if (currentRoundScale == endRoundScale)
            {
                leftEffect.transform.localScale = initPosition;
                leftHand.SetActive(false);
                isLeftAttack = false;
            }
            return;
        }


        if (isRightAttack)
        {
            leftHand.SetActive(false);
            rightHand.SetActive(true);

            /// leftEffect�� local position�� �ø���.
            float currentScale = Mathf.Lerp(rightEffect.transform.localScale.x, endPosition.x, Time.deltaTime * speed);
            rightEffect.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            float currentRoundScale = (float)System.Math.Round(currentScale, 2);
            float endRoundScale = (float)System.Math.Round(endPosition.x, 2);

            if (currentRoundScale == endRoundScale)
            {
                rightEffect.transform.localScale = initPosition;
                rightHand.SetActive(false);
                isRightAttack = false;
            }
        }
        return;
    }


    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                    leftHand.SetActive(false);
                    rightHand.SetActive(false);
                }
                break;
            case EventType.Tutorial:
                {
                    TutorialWeapon tutorialWeapon = (TutorialWeapon)param;
                    isTutorial = tutorialWeapon.isVPWeapon;
                }
                break;
        }
    }

}
