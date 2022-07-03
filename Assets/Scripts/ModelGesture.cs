using System;
using DigitalRubyShared;
using UnityEngine;

public class ModelGesture : MonoBehaviour
{
    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private TapGestureRecognizer tripleTapGesture;
    private SwipeGestureRecognizer swipeGesture;
    private PanGestureRecognizer panGesture;
    private ScaleGestureRecognizer scaleGesture;
    private RotateGestureRecognizer rotateGesture;
    private LongPressGestureRecognizer longPressGesture;

    private void Start()
    {
        CreateTapGesture();
        CreateSwipeGesture();
        CreatePanGesture();
        CreateScaleGesture();
        CreateRotateGesture();
    }

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void CreateRotateGesture()
    {
        rotateGesture = new RotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);
    }


    private void CreateScaleGesture()
    {
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }

    private void ScaleGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            Debug.LogFormat("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX,
                scaleGesture.FocusY);
            this.transform.localScale *= scaleGesture.ScaleMultiplier;
        }
    }

    private void CreatePanGesture()
    {
        panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 2;
        panGesture.StateUpdated += PanGestureCallback;
        FingersScript.Instance.AddGesture(panGesture);
    }

    private void CreateSwipeGesture()
    {
        swipeGesture = new SwipeGestureRecognizer();
        swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
        swipeGesture.StateUpdated += SwipeGestureCallback;
        swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        FingersScript.Instance.AddGesture(swipeGesture);
    }

    private void RotateGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            this.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
        }
    }

    private void SwipeGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            HandleSwipe(gesture.FocusX, gesture.FocusY);
            Debug.LogFormat("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX,
                gesture.StartFocusY,
                gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);
        }
    }

    private void PanGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            Debug.LogFormat("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.FocusX, gesture.FocusY,
                gesture.DeltaX,
                gesture.DeltaY);

            float deltaX = panGesture.DeltaX / 25.0f;
            float deltaY = panGesture.DeltaY / 25.0f;
            Vector3 pos = this.transform.position;
            pos.x += deltaX;
            pos.y += deltaY;
            this.transform.position = pos;
        }
    }

    private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.LogFormat("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);

            Launch.Instance.OnTap(gesture);
        }
    }

    private void HandleSwipe(float endX, float endY)
    {
        Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
        Vector3 startWorld = Camera.main.ScreenToWorldPoint(start);
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(endX, endY));
        float distance = Vector3.Distance(startWorld, endWorld);
        startWorld.z = endWorld.z = 0.0f;

        RaycastHit2D[] collisions =
            Physics2D.CircleCastAll(startWorld, 10.0f, (endWorld - startWorld).normalized, distance);

        if (collisions.Length != 0)
        {
            Debug.Log("Raycast hits: " + collisions.Length + ", start: " + startWorld + ", end: " + endWorld +
                      ", distance: " + distance);

            Vector3 origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(swipeGesture.VelocityX, swipeGesture.VelocityY,
                Camera.main.nearClipPlane));
            Vector3 velocity = (end - origin);
            Vector2 force = velocity * 500.0f;

            foreach (RaycastHit2D h in collisions)
            {
                h.rigidbody.AddForceAtPosition(force, h.point);
            }
        }
    }
}