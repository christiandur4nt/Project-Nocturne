using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRopeAnim : MonoBehaviour {
    [HideInInspector] public Transform grappleTip;
    private Spring spring;
    private LineRenderer lineRenderer;
    private Vector3 currentGrapplePosition;
    private GrappleAbility grappleScript;

    [Header("Animation Variables")]
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve effectCurve;
    
    void Reset() {
        quality = 500;
        damper = 14;
        strength = 800;
        velocity = 15;
        waveCount = 3;
        waveHeight = 15;
    }

    void Awake() {
        spring = new Spring();
        spring.SetTarget(0);
        lineRenderer = GetComponent<LineRenderer>();
        grappleScript = GetComponent<GrappleAbility>();
        grappleTip = GameObject.Find("Grapple Tip").transform;
    }
    
    // Called after Update
    void LateUpdate() {
        DrawRope();
    }

    void DrawRope() {
        // Only render if player is currently grappling
        if (!grappleScript.IsGrappling()) {
            currentGrapplePosition = grappleTip.position;
            spring.Reset();
            if (lineRenderer.positionCount > 0)
                lineRenderer.positionCount = 0;
            return;
        }

        if (lineRenderer.positionCount == 0) {
            spring.SetVelocity(velocity);
            lineRenderer.positionCount = quality + 1;
        }
        
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = grappleScript.GetGrapplePoint();
        var gunTipPosition = grappleTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++) {
            var delta = i / (float) quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * effectCurve.Evaluate(delta);
            lineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
}