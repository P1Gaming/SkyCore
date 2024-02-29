using UnityEngine;

public class RotateInPlace : MonoBehaviour
{
    public float speed;
    public Vector3 direction;

    private void Update()
    {
        transform.Rotate(speed * Time.deltaTime * direction);
    }
}
