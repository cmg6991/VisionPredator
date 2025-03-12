using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using Cinemachine;
using UnityEngine.UIElements.Experimental;
using Unity.VisualScripting;


public class Player : MonoBehaviour
{
    public float speed;
    float originSpeed;
    float mouseSpeed;
    //float gravity;

    float hAxis;
    float vAxis;
    bool run;
    bool dash;
    bool jump = false;
    bool sit;
    bool sitUp;

    bool isRunning;
    bool isDashing;
    bool isJumping;
    bool isSitting;
    bool isSliding;

    Vector3 moveVec;
    Vector3 runVec;
    Vector3 dashVec;
    Vector3 slideVec;

    Rigidbody rigid;
    Camera mainCamera;

    public CinemachineVirtualCamera cinevirtual;
    Transform head;
    Vector3 originPos;
    Vector3 sitPos;

    public PBullet bullet;
    bool npcDied;

    float attackSpeed = 5f;
    int absorptionCount;

    float doubleTapTime = 3f;
    int tapCount = 0;
    float lastTapTime = 0f;
    bool isCombo;
    bool isCollision;

    Transform direction;

    // ui�� �ѱ� bool
    public bool isDarkvision;
    private void Awake()
    {
        //gravity =  9.81f;
        moveVec = Vector3.zero;
        head = GameObject.Find("Head").GetComponent<Transform>();
    }
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        originPos = head.position - transform.position;
        sitPos = new Vector3(originPos.x, originPos.y - 0.8f, originPos.z);
        originSpeed = speed;
        direction = GameObject.Find("Direction").GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        GetInput();
        //if(dash && !isDashing)
        //    Dash();
        //Sliding();
        if (Input.GetKeyDown(KeyCode.E))
        {
            Change();
        }
        Sit();
        SitOut();
        Adrenaline();

        //if (!isCombo && dash)
        //{
        //    print("�������ٳన");
        //    if (Time.time - lastTapTime < doubleTapTime)
        //    {
        //        tapCount++;
        //        lastTapTime = Time.time;
        //    }
        //     else
        //        tapCount = 0;
        //}

        //if (tapCount == 2)
        //{
        //    isCombo = true;
        //    print("combo");
        //    Dash();
        //    tapCount = 0;
        //    isCombo = false;
        //}
    }

    private void FixedUpdate()
    {
        Move();
        Run();
        if (jump && !isJumping)
        {
            Jump();
            jump = false;
        }
        
        DarkVision();
    }
    private void LateUpdate()
    {
        if (dash)
            if (!isDashing)
                Dash();
        if (sit && !isSliding)
            Sliding();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        run = Input.GetButton("Dash");
        jump = Input.GetKey(KeyCode.Space);
        dash = Input.GetKeyDown(KeyCode.Q);
        sit = Input.GetKey(KeyCode.LeftControl);
        sitUp = Input.GetKeyUp(KeyCode.LeftControl);
    }
    void Move()
    {
        // mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed;
        // mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed;
        // //mouseY = Mathf.Clamp(mouseY, -30f, 30f);
        //this.transform.localEulerAngles = new Vector3(0,mouseX, 0);

        //// Quaternion verticalRotation = Quaternion.Euler(-mouseY, 0f, 0f);

        //transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
        // //transform.rotation *= verticalRotation;
        // pov.m_VerticalAxis.Value += -mouseY;


        //transform.Rotate(Vector3.up, mouseX);
        //transform.Rotate(Vector3.left, mouseY);
        
        moveVec =
            direction.forward.normalized * vAxis
            + direction.right.normalized * hAxis;
        rigid.velocity = moveVec.normalized * speed;
        //transform.position += moveVec.normalized * Time.deltaTime * speed;
        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        //moveVec = transform.TransformDirection(moveVec);
        moveVec.y = 0f;
    }
    void Jump()
    {
        Debug.Log("jump");
        isJumping = true;
        rigid.AddForce(Vector3.up * 20, ForceMode.Impulse);
    }

    void Run()
    {
        if (!isRunning && run)
        {
            isRunning = true;
            runVec = moveVec;
            speed += 5f;
            Invoke("RunOut", 0.1f);
        }
    }
    void RunOut()
    {
        speed = originSpeed;
        isRunning = false;
    }

    void Sit()
    {
        if (!isSitting && sit&&!isSliding)
        {
            Debug.Log("�ɾƾ���");
            isSitting = true;
            head.position = transform.position + sitPos;
            speed /= 2;
        }
    }

    void SitOut()
    {
        if(isSitting && sitUp &&!isSliding)
        {
            head.position = transform.position + originPos;
            isSitting = false;
            speed *= 2;
        }
    }

    void Sliding()
    {
        if(vAxis != 0)
        {
            isSliding = true;
            slideVec = direction.forward.normalized * 5f;
            speed *= 2f;
            StartCoroutine(SlideOut());
        }
    }
    IEnumerator SlideOut()
    {
        float slideDuration = 0.1f;
        float elapsedTime = 0f;
        Vector3 slideDirection = slideVec.normalized;
        float slideSpeed = speed * 2f;

        while (elapsedTime < slideDuration)
        {
            // �����̵� ���� ���ȿ��� �����̵� �������� �ӵ��� ����
            //rigid.velocity = slideDirection * slideSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rigid.velocity = slideDirection * slideSpeed;
        // �����̵��� ���� �� �ӵ��� ������� �ǵ���
        speed /= 2f;
        isSliding = false;
    }
    void Dash()
    {
        if (isSitting)
        {
            head.position = transform.position + originPos;
            isSitting = false;
        }
        if(dash)
        {
            isDashing = true;
            dashVec = transform.position + direction.forward.normalized * 3f;
            StartCoroutine(DashTime());
        }
        if (hAxis == -1)
        {
            isDashing = true;
            dashVec = transform.position - direction.right.normalized * 3f;
            //speed *= 3;
            //Invoke("DashOut", 0.1f);
            StartCoroutine(DashTime());
        }
        if (hAxis == 1)
        {
            isDashing = true;
            dashVec = transform.position + direction.right.normalized * 3f;
            //speed *= 3;
            //Invoke("DashOut", 0.1f);
            StartCoroutine(DashTime());
        }
        if (vAxis == -1)
        {
            isDashing = true;
            dashVec = transform.position - direction.forward.normalized * 3f;
            //speed *= 3;
            //Invoke("DashOut", 0.1f);
            StartCoroutine(DashTime());
        }
        if (vAxis == 1)
        {
            isDashing = true;
            dashVec = transform.position + direction.forward.normalized * 3f;
            //speed *= 3;
            //Invoke("DashOut", 0.1f);
            StartCoroutine(DashTime());
        }


    }
    IEnumerator DashTime()
    {
        float dashDuration = 0.1f;
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;
        while(elapsedTime < dashDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, dashVec, (elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = dashVec;
        dash = false;
        isDashing = false;
    }
    void DashOut()
    {
        //speed /= 3;
        isDashing = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall")
        {
            isJumping = false;
        }
        //if (collision.gameObject.tag == "NPC")
        //{
        //    Destroy(collision.gameObject);
        //}
    }
    void Change()
    {
        RaycastHit hit;
        float maxDistance = 20f;
        //Debug.DrawRay(transform.position, mainCamera.transform.forward * 100f, Color.red);
        if (Physics.Raycast(transform.position, mainCamera.transform.forward, out hit))
        {
            Debug.DrawRay(transform.position, mainCamera.transform.forward * hit.distance, Color.red);
            // �ٶ󺸴� ����� NPC�� ��쿡�� �̵�
            if (hit.collider.CompareTag("NPC"))
            {
                if (hit.distance <= maxDistance)
                {
                    transform.position = hit.point;
                    absorptionCount++;
                }
            }
        }
    }

    void DarkVision()
    {
        if (!isCollision && absorptionCount == 5)
        {
            isCollision = true;

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), true);


            StartCoroutine(EnableCollisionAfterDelay(3f));
            //Invoke("OutDarkVision", 3f);
        }
    }
    IEnumerator EnableCollisionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ���� �ð� ���� ���

        print("�ٲ�ߵ�");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), false);
        isCollision = false;
        absorptionCount = 0;
    }

    void Adrenaline()
    {
        if(npcDied == true)
        {
            speed += attackSpeed;
            npcDied = false;
            if(speed >= 10)
            {
                //�ٸ��Լ� ȣ��
                VisionPredator();
            }
            Invoke("OutAdrenaline", 10f);
        }
    }
    void OutAdrenaline()
    {
        speed = 5f;
    }

    void VisionPredator()
    {
        isDarkvision = true;

        //�ϴ� ���� �ڵ� 
        bullet.GetComponent<SphereCollider>().isTrigger = true;

        //�Ҹ�Ÿ�� �ڵ�
        speed = 2f;

        Invoke("OutVision", 10f);
    }

    void OutVision()
    {
        isDarkvision = false;
        bullet.GetComponent<SphereCollider>().isTrigger = false;
    }

    /// <summary>
    /// View Frustum ������ ���� �ֳ� ���� 
    /// </summary>
    /// <param name="_transform">Target Transform</param>
    /// <returns></returns>
    public bool IsTargetVisible(Transform _transform)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        Camera camera = null;

        if (brain != null)
        {
            camera = brain.OutputCamera;
        }

        if (camera != null)
        {
            // camera���� Frustum space�� �����´�.
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);

            // Target Transform ��ġ�� �޾ƿ´�.
            var point = _transform.position;

            // Frustum space�� ���鼭 Target Transform ��ġ�� �˻��Ѵ�. 
            // 0���� ������ ���� �ۿ� �ִٴ� �ǹ̿��� false ���� �˻��ߴµ� ���� ����?
            // �׷� ���� �ȿ� �ִٴ� ���̴� true��.
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// �� ��ġ -> Target ��ġ���� Ray�� ���� �߰��� ��ü�� ������� false �� ������� true
    /// </summary>
    /// <param name="_transform">Target Transfrom</param>
    /// <returns></returns>
    public bool IsRayTarget(Transform _transform)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        Camera camera = null;

        if (brain != null)
            camera = brain.OutputCamera;

        if (camera != null)
        {
            // camera���� viewport Point�� �����´�.
            Vector3 viewportPoint = camera.WorldToViewportPoint(_transform.position);

            // viewport ���� Ray�� ��� �׸���.
            Ray ray = camera.ViewportPointToRay(viewportPoint);
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Ray�� ���µ� _transform�� �� ������� false
                if (hit.transform != _transform)
                {
                    return false;
                }
            }
        }
        // ������� true
        return true;
    }
}
