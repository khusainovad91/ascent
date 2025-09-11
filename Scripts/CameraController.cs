using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : NetworkBehaviour//Singleton<CameraController> 
{
    public static CameraController Instance;

    [field: SerializeField]
    public Camera MainCamera { get; private set; }
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float edgeScrollSpeed = 10f;
    public float edgeThreshold = 10f; // расстояние от края экрана для активации

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private float zoomLevel;
    private bool automaticMove = false;

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    //private void Start()
    //{
    //    zoomLevel = transform.position.y;
    //}

    private void LateUpdate()
    {
        if (automaticMove) return;

        HandleMovement();
        HandleRotation();
            //HandleZoom();
        
    }

    void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        // WASD управление
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        if (Input.GetKey(KeyCode.W)) direction += forward;
        if (Input.GetKey(KeyCode.S)) direction -= forward;
        if (Input.GetKey(KeyCode.A)) direction -= right;
        if (Input.GetKey(KeyCode.D)) direction += right;

        // Движение по краям экрана
        //if (Input.mousePosition.x >= Screen.width - edgeThreshold) direction += right;
        //if (Input.mousePosition.x <= edgeThreshold) direction -= right;
        //if (Input.mousePosition.y >= Screen.height - edgeThreshold) direction += forward;
        //if (Input.mousePosition.y <= edgeThreshold) direction -= forward;

        // Применяем движение
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleRotation()
    {
        float rotation = 0f;

        // Q и E для вращения
        if (Input.GetKey(KeyCode.Q)) rotation -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) rotation += rotationSpeed * Time.deltaTime;

        // Вращение средней кнопкой мыши
        if (Input.GetMouseButton(2))
        {
            rotation += Input.GetAxis("Mouse X") * rotationSpeed * 0.1f;
        }

        transform.LeanRotateAround(Vector3.up, rotation, 0);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (Input.GetKey(KeyCode.Equals)) scroll += zoomSpeed * Time.deltaTime;  // Клавиша "+"
        if (Input.GetKey(KeyCode.Minus)) scroll -= zoomSpeed * Time.deltaTime;   // Клавиша "-"

        zoomLevel -= scroll;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);


        Vector3 pos = transform.position;
        pos.y = zoomLevel;
        transform.position = pos;

    }
    public void SmoothLookAt(Transform target, float speed)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void MoveCameraToTargetRpc(NetworkObjectReference fieldObjectNOR, float duration) {
        fieldObjectNOR.TryGet(out NetworkObject fieldbjectNO);
        FieldObject fieldObject = fieldbjectNO.GetComponent<FieldObject>();
        StartCoroutine(MoveCameraToTarget(fieldObject.transform, duration));
    }

    public IEnumerator MoveCameraToTarget(Transform target, float duration)
    {
        LeanTween.cancel(this.gameObject);
        automaticMove = true;

        // Берем направление к цели в плоскости XZ (убираем Y)
        Vector3 directionXZ = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;

        // Смещаем камеру к цели, но без изменения высоты
        Vector3 targetPosition = target.position - directionXZ * Constants.CAMERA_STOP_DISTANCE;
        targetPosition.y = transform.position.y; // Оставляем текущую высоту

        // Поворачиваем камеру в сторону цели, но без наклона
        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.LeanMove(targetPosition, duration);
        transform.LeanRotateY(targetRotation.eulerAngles.y, duration).setOnComplete(() => LeanTween.delayedCall(0.1f, () => automaticMove = false));
        yield return new WaitForSeconds(duration);
    }

    [Rpc(SendTo.Everyone)]
    public void FollowTartgetRpc(NetworkObjectReference fieldObjectNOR)
    {
        automaticMove = true;
        fieldObjectNOR.TryGet(out NetworkObject fieldbjectNO);
        FieldObject fieldObject = fieldbjectNO.GetComponent<FieldObject>();
        StartCoroutine(FollowTargetSmoothTween(fieldObject.transform, 0.5f));
        //FollowTarget(fieldObject.transform);        
    }

    private IEnumerator FollowTargetSmoothTween(Transform target, float v)
    {
        LeanTween.cancel(gameObject);
        automaticMove = true;

        Vector3 directionXZ = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;
        Vector3 targetPosition = target.position - directionXZ * Constants.CAMERA_STOP_DISTANCE;
        targetPosition.y = transform.position.y;

        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Плавное перемещение и поворот
        LeanTween.move(gameObject, targetPosition, v).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotateY(gameObject, targetRotation.eulerAngles.y, v).setEase(LeanTweenType.easeInOutQuad);

        yield return new WaitForSeconds(v + 0.1f); // подстраховка
        automaticMove = false;
    }

    public void FollowTarget(Transform target)
    {
        automaticMove = true;
        FollowTargetDirect(target);
        //.setOnComplete(() => LeanTween.delayedCall(0.1f, 
        //() => automaticMoove = false));
        automaticMove = false;
    }

    private void FollowTargetDirect(Transform target)
    {
        LeanTween.cancel(this.gameObject);

        Vector3 directionXZ = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).
            normalized;
        Vector3 targetPosition = target.position - directionXZ * Constants.CAMERA_STOP_DISTANCE;
        targetPosition.y = transform.position.y;

        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.LeanMove(targetPosition, 0);

        transform.LeanRotateY(targetRotation.eulerAngles.y, 0);
    }

    [Rpc(SendTo.Everyone)]
    public void SetAutomaticMoveRpc(bool automaticMove)
    {
        this.automaticMove = automaticMove;
    }

}
