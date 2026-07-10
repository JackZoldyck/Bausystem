using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;

    [Header("Wind")]
    public Vector3 windDirection = new Vector3(1f, 0f, 0f);
    public float windStrength = 5f;

    [Header("Langsame Änderungen")]
    public bool changeWindOverTime = true;
    public float directionChangeSpeed = 5f;
    public float strengthChangeSpeed = 0.5f;
    public float minWindStrength = 2f;
    public float maxWindStrength = 8f;

    private float targetAngle;
    private float targetStrength;
    private float changeTimer;

    void Awake()
    {
        Instance = this;

        windDirection.y = 0f;
        windDirection.Normalize();

        targetAngle = Mathf.Atan2(windDirection.z, windDirection.x)
                      * Mathf.Rad2Deg;

        targetStrength = windStrength;
    }

    void Update()
    {
        if (!changeWindOverTime)
            return;

        changeTimer -= Time.deltaTime;

        if (changeTimer <= 0f)
        {
            targetAngle = Random.Range(0f, 360f);
            targetStrength = Random.Range(
                minWindStrength,
                maxWindStrength
            );

            changeTimer = Random.Range(20f, 50f);
        }

        float currentAngle =
            Mathf.Atan2(windDirection.z, windDirection.x)
            * Mathf.Rad2Deg;

        currentAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            directionChangeSpeed * Time.deltaTime
        );

        windStrength = Mathf.MoveTowards(
            windStrength,
            targetStrength,
            strengthChangeSpeed * Time.deltaTime
        );

        float radians = currentAngle * Mathf.Deg2Rad;

        windDirection = new Vector3(
            Mathf.Cos(radians),
            0f,
            Mathf.Sin(radians)
        );
    }

    public Vector3 GetWindVelocity()
    {
        return windDirection.normalized * windStrength;
    }
}