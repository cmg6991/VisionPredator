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

    // 초기 값
    public readonly Vector3 initPosition = new Vector3(0.3f, 0.3f, 0.3f);
    private readonly Vector3 endPosition = new Vector3(1.0f, 1.0f, 1.0f);

    private void Start()
    {
        // leftHand에서 localTransform을 조절해야 한다. -> 점점 커지게
        // 안에 코드도 없는데 어떻게 자식에서 VP State를 가져오지? GetComponentInChild를 하면 자기 자신 포함해서 가져온다. 

        // 부모에는 없고 자식에는 있는 거로 이용해서 GameObject를 불러오자.
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

            /// leftEffect의 local position을 늘린다.
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

            /// leftEffect의 local position을 늘린다.
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
