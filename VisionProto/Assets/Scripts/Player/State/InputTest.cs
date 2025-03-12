using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour, IListener
{
    public float hAxis;
    public float vAxis;

    public bool isSit;
    public bool isGrounded;
    public bool isOnSlope;
    public bool isJump;
    public bool isDashed;
    public bool isSlopeJump;
    public bool isSlash;
    public bool isRay;

    public int dashIndex = 0;

    public float playerScale;

    public bool dashDamage;
    public bool slashDamage;

    private Vector3 screenCrossHair;
    private int saveScreenWidth;
    private int saveScreenHeight;
    private GameObject previousGameObject;
    private bool isVPState;

    private float radius = 0.05f;
    private float offset = 0.1f;
    private bool drawGizmo;

    private Transform groundCheck;
    float rayLength;

    public bool explictSit;

    public float objectDistance = 2f;
    public float gunDistance = 5f;
    public float grapplingDistance = 10f;

    private void Start()
    {
        playerScale = this.transform.localScale.y;
        groundCheck = GameObject.Find("groundCheck").GetComponent<Transform>();
        UpdateScreenSize();
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    private void Update()
    {
        if (Screen.width != saveScreenWidth || Screen.height != saveScreenHeight)
            UpdateScreenSize();

        GetInput();
        RaycastWeapon();
        //RaycastObject();
        DataManager.Instance.playerPosition = this.transform;
        EventManager.Instance.NotifyEvent(EventType.PlayerPosition, this.transform.position);
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
    }
    public void CheckGround()
    {
        //LayerMask playerLayerMask = ~LayerMask.GetMask("Default", "NPC");
        //// �������� ������ ��ü�� ��ġ
        //Vector3 pos = transform.position + Vector3.up*offset;
        //// CheckSphere�� ����Ͽ� ��ü�� groundLayer�� �浹�ϴ��� Ȯ��
        //isGrounded = Physics.CheckSphere(pos , radius, playerLayerMask);
        if (isRay)
            rayLength = 1.1f;
        else
            rayLength = 1.1f;

        RaycastHit hit;
        LayerMask playerLayerMask = ~LayerMask.GetMask("Default", "NPC", "NormalNPC", "StencilNPC", "DeadNPC", "CCTVArea", "Outline");
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, playerLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

        //RaycastHit hit;

        //LayerMask playerLayerMask = ~LayerMask.GetMask("Default", "NPC");
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, playerLayerMask);
        //Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);
        //Debug.Log("���� ��� ��");
    }

    private void UpdateScreenSize()
    {
        screenCrossHair = new Vector3(Screen.width / 2, Screen.height / 2);
        saveScreenWidth = Screen.width;
        saveScreenHeight = Screen.height;
    }

    private void RaycastWeapon()
    {
        // Ray�� ���µ� �ش� Object�� Gun�̸� Outline�� �߰�, ���ڰ� �߰� �ϰ� 
        // Tag�� �ϸ� ���޾� ������ �ȵǰ� �߰����� �װ� �ʿ��ϴ�.
        // Gun���� ��� �ִµ� 
        Ray ray = Camera.main.ScreenPointToRay(screenCrossHair);
        RaycastHit hit;
        LayerMask npcDectorLayerMask = ~LayerMask.GetMask("DeadNPC", "StencilNPC", "Player", "StackingCamera", "CCTVArea");
        OutlineWeapon weapon;
        OutlineObject outlineObject;

        bool isObjectEnter;
        bool isGrapplingEnter;

        if (Physics.Raycast(ray, out hit, 100f, npcDectorLayerMask))
        {
            float distance = Vector3.Distance(this.transform.position, hit.point);
            // Player StateMachine 
            if (distance <= objectDistance)
                isObjectEnter = true;
            else
                isObjectEnter = false;

            if (distance <= grapplingDistance)
                isGrapplingEnter = true;
            else
                isGrapplingEnter = false;


            if (hit.collider.gameObject.tag != "Gun" &&
                hit.collider.gameObject.tag != "Cabinet" && hit.collider.gameObject.tag != "Item" && hit.collider.gameObject.tag != "Grappling" && hit.collider.gameObject.tag != "Button")
            {
                if (previousGameObject != null)
                {
                    weapon = previousGameObject.GetComponent<OutlineWeapon>();
                    if (weapon != null)
                        weapon.MouseExit();

                    outlineObject = previousGameObject.GetComponent<OutlineObject>();
                    if (outlineObject != null)
                    {
                        if (!isVPState)
                            outlineObject.MouseExit();
                        else
                            outlineObject.VPMouse();
                    }
                }
                previousGameObject = null;
                return;
            }

            // Layer Gun�� ��
            GameObject currentGameObject = hit.collider.gameObject;

            if (currentGameObject != previousGameObject)
            {
                if (previousGameObject != null)
                {
                    // Mouse Exit ȣ��
                    weapon = previousGameObject.GetComponent<OutlineWeapon>();
                    if (weapon != null)
                        weapon.MouseExit();

                    outlineObject = previousGameObject.GetComponent<OutlineObject>();
                    if (outlineObject != null)
                        outlineObject.MouseExit();
                }

                // Mouse Enter ȣ��
                weapon = currentGameObject.GetComponent<OutlineWeapon>();
                if (weapon != null)
                {
                    weapon.MouseEnter();

                    if (isObjectEnter)
                        weapon.PrintWeaponName();
                    else
                        weapon.BlankWeaponName();
                }

                outlineObject = currentGameObject.GetComponent<OutlineObject>();
                if (outlineObject != null)
                {
                    if(!isVPState)
                        outlineObject.MouseEnter();
                    else
                        outlineObject.VPMouse();

                    if (currentGameObject.tag != "Grappling")
                    {
                        if (isObjectEnter)
                            outlineObject.PrintObjectName();
                        else
                            outlineObject.PrintBlankObjectName();
                    }
                }
                previousGameObject = currentGameObject;
            }

            // Mouse Over ȣ��
            weapon = currentGameObject.GetComponent<OutlineWeapon>();
            if (weapon != null)
            {
                weapon.MouseOver();

                if (isObjectEnter)
                    weapon.PrintWeaponName();
                else
                    weapon.BlankWeaponName();
            }

            outlineObject = currentGameObject.GetComponent<OutlineObject>();
            if (outlineObject != null)
            {
                if(!isVPState)
                    outlineObject.MouseOver();
                else
                    outlineObject.VPMouse();

                if (currentGameObject.tag != "Grappling")
                {
                    if (isObjectEnter)
                        outlineObject.PrintObjectName();
                    else
                        outlineObject.PrintBlankObjectName();
                }
            }
        }
        else
        {
            // Mouse Exit ȣ��
            if (previousGameObject != null)
            {
                weapon = previousGameObject.GetComponent<OutlineWeapon>();
                if (weapon != null)
                    weapon.MouseExit();

                outlineObject = previousGameObject.GetComponent<OutlineObject>();
                if (outlineObject != null)
                    outlineObject.MouseExit();

                previousGameObject = null;
            }
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (dashDamage)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damaged(50, transform.position, transform.position, this.gameObject);
                }
            }
        }

        if (slashDamage)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damaged(35, transform.position, transform.position, this.gameObject);
                }
            }
        }
        if (collision.gameObject.CompareTag("SitWall"))
        {
            explictSit = true;
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.CompareTag("SitWall"))
    //    {
    //        explictSit = true;
    //        Debug.Log("stay");
    //    }
        

    //}
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SitWall"))
        {
            explictSit = false;
            Debug.Log("exit");
        }
        
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.CompareTag("SitWall"))
    //    {
    //        explictSit = true;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("SitWall"))
    //    {
    //        explictSit = false;
    //    }
    //}

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                }
                break;
        }
    }

}