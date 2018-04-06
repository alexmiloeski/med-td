using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // How long the object should shake for.
    private static float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private static float startShakeAmount = 0.2f;
    private static float shakeAmount = 0.2f;
    private static float decreaseFactor = 2.0f;
    private static float decrease = 1f;

    //private Transform camTransform;
    private static Vector3 originalPos;

    private static bool shaking = false;
    private static float decrement = 0f;

    void Start ()
    {
        //camTransform = transform;
        originalPos = transform.localPosition;
    }
	
	void Update ()
    {
        if (shaking)
            ShakeCamera();
	}

    private void ShakeCamera()
    {
        if (shakeDuration > 0)
        {
            float newShakeAmount = shakeAmount - (decrement * Time.deltaTime);
            if (newShakeAmount <= startShakeAmount)
                shakeAmount = newShakeAmount;
            
            decrease += Time.deltaTime;
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime;// * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    /// <summary> Shake the camera for <paramref name="duration"/> seconds, with
    /// <paramref name="_startShakeAmount"/> intensity. Values between 0.03f and
    /// 0.5f should be enough for <paramref name="_startShakeAmount"/>. </summary>
    internal static void StartShaking(float duration, float _startShakeAmount)
    {
        startShakeAmount = _startShakeAmount;
        decrement = startShakeAmount / duration;
        shakeAmount = startShakeAmount;
        shakeDuration = duration;
        decrease = 1f;
        shaking = true;
    }
}
