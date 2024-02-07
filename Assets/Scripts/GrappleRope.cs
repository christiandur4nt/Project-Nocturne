using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    [SerializeField] private float animationDuration = 5f;
    private LineRenderer lineRenderer;
    public Grapple grapple;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private IEnumerator AnimateLine() {
        float startTime = Time.time;

        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);
        
        Vector3 pos = startPos;
        while (pos != endPos) {
            float t = (Time.time - startTime) / animationDuration;
            pos = Vector3.Lerp(startPos, endPos, t);
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(pos));
            yield return null;
        }
    }
}
