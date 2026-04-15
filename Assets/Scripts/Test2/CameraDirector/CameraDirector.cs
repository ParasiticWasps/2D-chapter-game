using UnityEngine;
using System.Collections;

public class CameraDirector : MonoBehaviour
{
    public Transform cameraRig;
    public Camera mainCamera;



    private void Start()
    {

    }

    public void Play(CameraActionProfile profile)
    {
        Debug.Log("CameraDirector 被调用: " + profile.name);
        StopAllCoroutines();
        StartCoroutine(Execute(profile));
    }

    //IEnumerator Execute(CameraActionProfile profile)
    //{
    //    Vector3 startPos = cameraRig.position;
    //    Vector3 targetPos = startPos;

    //    switch (profile.actionType)
    //    {
    //        case CameraActionType.PushIn:
    //            float startSize = mainCamera.orthographicSize;
    //            float targetSize = startSize - profile.pushDistance;

    //            float t2 = 0;
    //            while (t2 < profile.duration)
    //            {
    //                t2 += Time.deltaTime;
    //                float lerp = t2 / profile.duration;
    //                mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, lerp);
    //                yield return null;
    //            }
    //            yield break;

    //        case CameraActionType.Pan:
    //            targetPos += profile.panOffset;
    //            break;

    //        case CameraActionType.FocusTarget:
    //            if (profile.focusTarget != null)
    //            {
    //                targetPos = new Vector3(
    //                    profile.focusTarget.position.x,
    //                    profile.focusTarget.position.y + profile.focusOffset,
    //                    startPos.z
    //                );
    //            }
    //            break;
    //    }

    //    float time = 0;
    //    float duration = profile.duration;

    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = time / duration;

    //        cameraRig.position = Vector3.Lerp(startPos, targetPos, t);
    //        yield return null;
    //    }
    //}
    IEnumerator Execute(CameraActionProfile profile)
    {
        Debug.Log("开始移动镜头");

        Vector3 startPos = cameraRig.position;
        Vector3 targetPos = startPos + new Vector3(5, 0, 0);

        float time = 0;
        while (time < 2f)
        {
            time += Time.deltaTime;
            cameraRig.position = Vector3.Lerp(startPos, targetPos, time / 2f);
            yield return null;
        }

        Debug.Log("移动完成");
    }
}