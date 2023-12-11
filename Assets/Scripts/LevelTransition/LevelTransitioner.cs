using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Map;

namespace Milan.GrassBubble.LevelTransition
{
    public class LevelTransitioner : MonoBehaviour
    {

        const int childIndexOfMapModel = 0;
        const int swapLocationLength = 6;
        public GameObject mapTemplate;
        public Camera cam;

        [SerializeField,Range(0f,5f)]
        public float timeToWaitAfterCamFinish = 1.5f;
        [SerializeField,Range(1.25f,3)]
        public float centerScaleFactor = 1.25f;
        [Range(1.25f,3)]
        public float placeSwapDuration = 1;
        [SerializeField,Range(4f,7f)]
        float fadeStartTime;
        [SerializeField,Range(0.1f,5f)]
        float fadeDuration;
        public AnimationCurve animationCurve;

        // public Transform mapTransform;
        // public LevelList levelList;
        Transform[] transformPoints;
        Vector3[] points;
        public delegate void Notify();
        public static event Notify SwapHalfWayComplete;
        public static event Notify CamAnimationComplete;

        void Awake() 
        {
            Plane plane = new Plane(Vector3.up,0);
            
            float heightMid = Screen.height / 2;
            float heightLowerMid = heightMid / 2;
            float widthMid = Screen.width / 2;
            float widthQuart = Screen.width / 4;
            float left = widthQuart;
            float right = Screen.width - widthQuart;

            Vector2 midScreenPoint = new Vector2(widthMid,heightLowerMid);
            Vector2 leftScreenPoint = new Vector2(left,heightLowerMid);
            Vector2 rightScreenPoint = new Vector2(right,heightLowerMid);

            Vector3 midPoint = GetRayPositionFromScreen(midScreenPoint);
            Vector3 leftPoint = GetRayPositionFromScreen(leftScreenPoint);
            Vector3 rightPoint = GetRayPositionFromScreen(rightScreenPoint);

            float xDistance = Vector3.Distance(midPoint,rightPoint);
            transformPoints = new Transform[swapLocationLength];
            points = new Vector3[swapLocationLength];
            points[0] = new Vector3(leftPoint.x * 2,0,midPoint.z);
            points[1] = leftPoint;
            points[2] = midPoint;
            points[3] = rightPoint;
            points[4] = new Vector3(rightPoint.x * 2,0,midPoint.z);
            points[5] = new Vector3(rightPoint.x * 4,0,midPoint.z);
        }

        public void Initialize(LevelList levelList,int startingLevel)
        {
            int currentLevel = startingLevel - 2;
            for (int i = 0; i < points.Length; i++)
            {
                Debug.Log($"index:{i}/currentLevel{currentLevel}/{levelList.Length}");
                if(currentLevel < 0 || currentLevel > levelList.Length - 1)
                {
                    Debug.Log($"SKIP AT {currentLevel}");
                    currentLevel++;
                    continue;
                }
                LevelData levelData = levelList.GetLevelData(currentLevel);
                MapGenerator mapGenerator = Instantiate(mapTemplate,Vector3.zero,Quaternion.identity).transform.GetChild(childIndexOfMapModel).GetComponent<MapGenerator>();
                if(currentLevel == levelList.Length - 1)
                    mapGenerator.SpawnQuestionMark();
                else
                    mapGenerator.Initialize(levelData,false);
                mapGenerator.transform.parent.position = points[i];
                mapGenerator.transform.parent.name = levelData.name;
                if(i == 2)
                {
                    mapGenerator.transform.localScale = mapGenerator.transform.localScale * centerScaleFactor;
                }
                transformPoints[i] = mapGenerator.transform.parent;
                currentLevel++;
            }    
            StartLevelTransition(currentLevel);
        }
        public void StartLevelTransition(int currentLevel)
        {
            int index = 0;
            foreach (Transform item in transformPoints)
            {
                if(item == null)
                {
                    index++;
                    continue;
                }
                StartCoroutine(LevelSwitch(item,index));
                index++;
            }
            StartCoroutine(CameraPan());
            StartCoroutine(Fade());
        }

        IEnumerator LevelSwitch(Transform levelTransform, int index)
        {
            Transform child = levelTransform.GetChild(childIndexOfMapModel);
            int targetIndex = index;
            float t = 0f;
            Vector3 originalPos = levelTransform.position;
            Vector3 locationToSwapTo = levelTransform.position;
            targetIndex -= 1;
            if(targetIndex >= 0 && targetIndex <= swapLocationLength)
            {
                locationToSwapTo = points[targetIndex];
            }
            if(targetIndex < 0)
            {
                locationToSwapTo = new Vector3(originalPos.x,0,originalPos.z - 10);
            }
            Vector3 oldScale = child.localScale;
            Vector3 newScale = child.localScale;


            if(index == 2)
            {
                newScale = Vector3.one * 0.5f;
            }
            if(index == 3)
            {
                newScale = Vector3.one * 0.5f * centerScaleFactor;
            }
            //Debug.Log("Changing Rotation");
            yield return new WaitForSeconds(1.0f);
            while(t < placeSwapDuration)
            {
                float finalPercent = Mathf.Clamp01(t / placeSwapDuration);
                float curvePercent = EasingUtil.EaseOutQuart(finalPercent);
                levelTransform.position = Vector3.Lerp(originalPos,locationToSwapTo, curvePercent);
                child.localScale = Vector3.Lerp(oldScale,newScale, curvePercent);
                if(finalPercent > 0.1f)
                    SwapHalfWayComplete?.Invoke();
                yield return null;
                t += Time.deltaTime;
            }
            levelTransform.position = locationToSwapTo;
            
            yield return null;
        }
        IEnumerator CameraPan()
        {
            Vector3 currentPosition = cam.transform.position;
            Vector3 targetPosition = new Vector3(points[3].x,points[3].y + 0.5f,points[3].z - 1.5f); //Center position

            float t = 0;
            yield return new WaitForSeconds(4);
            while(t < placeSwapDuration)
            {
                float finalPercent = Mathf.Clamp01(t / placeSwapDuration);
                float curveZPercent = EasingUtil.EaseInBack(finalPercent);
                float curveYPercent = animationCurve.Evaluate(finalPercent);

                float zTrans = Mathf.LerpUnclamped(currentPosition.z,targetPosition.z,curveZPercent);
                float yTrans = Mathf.LerpUnclamped(currentPosition.y,targetPosition.y,1 - curveYPercent);

                cam.transform.position = new Vector3(cam.transform.position.x,yTrans,zTrans);
                yield return null;
                t += Time.deltaTime;
            }
            yield return new WaitForSeconds(timeToWaitAfterCamFinish);
            CamAnimationComplete?.Invoke();
        }
        IEnumerator Fade()
        {
            yield return new WaitForSeconds(fadeStartTime);
            FadeController.Fade(FadeController.FadeColor.Clear,
                FadeController.FadeColor.White,
                FadeController.FadeType.EaseInOutQuad,
                fadeDuration);
        }

        // void OnDrawGizmos() 
        // {
        //     // Debug.Log(cam.pixelHeight);
        //     Plane plane = new Plane(Vector3.up,0);
            
        //     float heightMid = Screen.height / 2;
        //     float left = 0;
        //     float right = Screen.width;
        //     float widthMid = Screen.width / 2;

        //     Vector2 midScreenPoint = new Vector2(widthMid,heightMid);
        //     Vector2 leftScreenPoint = new Vector2(left,heightMid);
        //     Vector2 rightScreenPoint = new Vector2(right,heightMid);

        //     Vector3 midPoint = GetRayPositionFromScreen(midScreenPoint);
        //     Vector3 leftPoint = GetRayPositionFromScreen(leftScreenPoint);
        //     Vector3 rightPoint = GetRayPositionFromScreen(rightScreenPoint);

        //     float xDistance = ((Mathf.Abs(rightPoint.x) + Mathf.Abs(leftPoint.x) ) / 2) - midPoint.x;

        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireCube(leftPoint,Vector3.one * 2);
        //     Gizmos.DrawWireCube(rightPoint,Vector3.one * 2);

        // }
        Vector3 GetRayPositionFromScreen(Vector2 screenPos)
        {
            Plane plane = new Plane(Vector3.up,0);
            float d = 0;
            Ray ray = cam.ScreenPointToRay(screenPos);
            Vector3 leftPoint = Vector3.positiveInfinity;
            Vector3 rightPoint = Vector3.positiveInfinity;
            if(plane.Raycast(ray,out d))
                return ray.GetPoint(d);
            else 
                return Vector3.positiveInfinity;
        }
    }
}