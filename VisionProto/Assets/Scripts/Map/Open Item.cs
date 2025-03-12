using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenItem : MonoBehaviour
{
    public GameObject itemTotal;
    public GameObject cover;
    public GameObject itemCase;
    public bool isControl;
    public float openSpeed;

    private Quaternion openItemDegree;

    private bool isOpen;    // item�� �̹� ���ȴٸ� üũ


    // �� ���� �̸���? �� �����ӿ� cover.transform.localRotaion�� �˻��ϰ� �ٷ� �ݴ´�.
    private bool currentLocalRotation;

    public GameObject gun1;
    public GameObject gun2;
    public GameObject gun3;

    private GameObject gun;
    private Vector3 gunScale;

    // Start is called before the first frame update
    void Start()
    {
        openItemDegree = Quaternion.Euler(-90f, 0, 90f);

        currentLocalRotation = false;

        // Random ������ �� �� �ϳ� ��µǰ� �ϱ�

        // ������ ���� ���ܵȴ�. 0 ~ 2���� ���̶� ��
        int startNumber = 0;
        int finalNumber = 3;
        int randomNumber = Random.Range(startNumber, finalNumber); 
        
        switch (randomNumber) 
        {
            case 0:
                gun = gun1;
                gunScale = Vector3.one * 2f;
                break;
            case 1:
                gun = gun2;
                gunScale = Vector3.one;
                break;
            case 2:
                gun = gun3;
                gunScale = Vector3.one * 2f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isControl)
        {
            if(!currentLocalRotation)
            {
                if (cover.transform.localRotation.eulerAngles.y == 180)
                    isOpen = false;
                else 
                    isOpen = true;

                currentLocalRotation = true;
            }


            cover.transform.localRotation = Quaternion.Lerp(cover.transform.localRotation, openItemDegree, Time.deltaTime * openSpeed);

            if(cover.transform.localRotation.eulerAngles.y <= 100)
            {
                // item ���� 
                if (!isOpen)
                {
                    Vector3 position = itemCase.transform.position;

                    Vector3 forward = -itemCase.transform.right;
                    position.y += 0.2f;

                    float rotationAngle = itemCase.transform.rotation.eulerAngles.y;
                    Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

                    GameObject createObject = Object.Instantiate(gun, position, rotation);
                    createObject.transform.localScale = gunScale * 0.075f;
                    Rigidbody rigidbody = createObject.GetComponentInChildren<Rigidbody>();
                    rigidbody.velocity = new Vector3(0, 5f, 0) + forward;

                    isOpen = true;

                    int powLayer = LayerMask.GetMask("Wall");
                    int wallLayer = (int)Mathf.Ceil(Mathf.Log(powLayer) / Mathf.Log(2));

                    cover.GetComponent<InteractionObject>().isDone = true;
                    itemCase.GetComponent<InteractionObject>().isDone = true;

                    cover.layer = wallLayer;
                    cover.tag = "Wall";
                    itemCase.layer = wallLayer;
                    itemCase.tag = "Wall";
                }
            }
        }
    }
}
