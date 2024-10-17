using UnityEngine;

public class Door : MonoBehaviour
{
    public enum OpenType { RotToOpen, MoveToOpen }
    public OpenType openType;

    public enum OpenAxis { X, Y, Z }
    public OpenAxis doorAxis;

    public bool onlyOpen;
    public bool canBeOpenedNow;

    private bool isOpen;
    public float openSpeed = 150f;
    public float openDistanceOrAngle = 140f;

    public AudioSource moveOrRotateSound;
    public AudioSource openSound;
    public AudioSource closeSound;
    public AudioSource notOpeningSound;

    public GameObject doorHandle;

    public enum HandleAxis { X, Y, Z }
    public HandleAxis handleAxis;

    public float handleRotationAngle = -45f;

    private Quaternion handleStartRotation;
    private float startDistanceOrAngle;
    private bool openCloseOn;

    public GameObject interactionImage;

    public float interactionDistance = 3f; // Максимальное расстояние для взаимодействия
    public Transform player; // Ссылка на игрока

    void Start()
    {
        if (openType == OpenType.MoveToOpen)
        {
            if (doorAxis == OpenAxis.X) startDistanceOrAngle = transform.localPosition.x;
            else if (doorAxis == OpenAxis.Y) startDistanceOrAngle = transform.localPosition.y;
            else if (doorAxis == OpenAxis.Z) startDistanceOrAngle = transform.localPosition.z;
        }
        else
        {
            if (doorAxis == OpenAxis.X) startDistanceOrAngle = transform.localEulerAngles.x;
            else if (doorAxis == OpenAxis.Y) startDistanceOrAngle = transform.localEulerAngles.y;
            else if (doorAxis == OpenAxis.Z) startDistanceOrAngle = transform.localEulerAngles.z;
        }

        if (doorHandle) handleStartRotation = doorHandle.transform.localRotation;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Проверяем, находится ли игрок в пределах расстояния взаимодействия
        if (distance <= interactionDistance)
        {
            interactionImage.SetActive(true); // Показываем изображение взаимодействия

            // Проверяем нажатие клавиши "F"
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenClose();
            }
        }
        else
        {
            interactionImage.SetActive(false); // Скрываем изображение взаимодействия
        }

        // Логика открытия/закрытия двери
        if (openCloseOn)
        {
            if (isOpen)
            {
                if (openType == OpenType.MoveToOpen)
                {
                    MoveDoor(true);
                }
                else
                {
                    RotateDoor(true);
                }
            }
            else
            {
                if (openType == OpenType.MoveToOpen)
                {
                    MoveDoor(false);
                }
                else
                {
                    RotateDoor(false);
                }
            }
        }
    }

    private void MoveDoor(bool opening)
    {
        float targetPosition = startDistanceOrAngle + (opening ? openDistanceOrAngle : 0);
        float newPosition = Mathf.MoveTowards(transform.localPosition[(int)doorAxis], targetPosition, openSpeed * Time.deltaTime);
        Vector3 newLocalPosition = transform.localPosition;
        newLocalPosition[(int)doorAxis] = newPosition;
        transform.localPosition = newLocalPosition;

        if (Mathf.Approximately(transform.localPosition[(int)doorAxis], targetPosition))
        {
            StopOpenClose();
        }
    }

    private void RotateDoor(bool opening)
    {
        float targetAngle = startDistanceOrAngle + (opening ? openDistanceOrAngle : 0);
        float newAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles[(int)doorAxis], targetAngle, openSpeed * Time.deltaTime);
        Vector3 newLocalEulerAngles = transform.localEulerAngles;
        newLocalEulerAngles[(int)doorAxis] = newAngle;
        transform.localEulerAngles = newLocalEulerAngles;

        if (Mathf.Approximately(transform.localEulerAngles[(int)doorAxis], targetAngle))
        {
            StopOpenClose();
        }
    }

    public void OpenClose()
    {
        if (canBeOpenedNow)
        {
            if (moveOrRotateSound) moveOrRotateSound.Play();
            openCloseOn = true;

            isOpen = !isOpen; // Переключаем состояние двери if (isOpen)
            {
                if (openSound) openSound.Play();
                if (onlyOpen)
                {
                    gameObject.tag = "Untagged";
                    interactionImage.SetActive(false);
                }
            }
        }
        else
        {
            if (notOpeningSound) notOpeningSound.Play();
            Debug.Log("ЗАКРЫТО !!!");
        }
    }

    void StopOpenClose()
    {
        openCloseOn = false;
        if (moveOrRotateSound) moveOrRotateSound.Stop();
        if (closeSound && !isOpen) closeSound.Play();
    }
}