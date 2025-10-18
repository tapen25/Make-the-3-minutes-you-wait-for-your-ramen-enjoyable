using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class TapToPlayAnimation : MonoBehaviour
{
    private Camera arCamera;
    private Animator animator;

    void Start()
    {
        arCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == this.transform)
                    {
                        // Animator の Trigger をセット
                        animator.SetTrigger("PlayAnimation");
                    }
                }
            }
        }
    }
}
