using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Milan.GrassBubble.Gameloop
{
    public class CameraController : MonoBehaviour
    {   
        const int LeftClick = 0;
        Vector3 cameraTargetPos;
        Vector2 cameraRotateDelta;
        [SerializeField]
        float cameraDistance = 20;
        [SerializeField]
        public AnimationCurve introRotationCurve;
        [SerializeField]
        float minZoomDistance = 2;
        [SerializeField]
        float maxZoomDistance = 10;
        [SerializeField]
        float beginningHeight = 20;
        Camera cam;
        [SerializeField]
        Transform cameraTarget;
        [SerializeField]
        float animHoverDownTime = 2;
        public Transform cursor;
        Vector2 previousMousePos;
        Vector2 currentMousePos;

        [SerializeField]
        Vector2 mouseDelta;
        [SerializeField]
        Vector2 cameraVelocity;
        Vector2Int previousGridPoint;
        Vector2Int currentGridPoint;
        public bool canControl = false;
        public BubbleSpawn bubbleSpawn;
        [SerializeField]
        float zoomFactor;
        [SerializeField]
        float camSensitivity;
        public GameObject cursorGO;
        const int cursorReticleIndex = 1;
        [SerializeField]
        bool isHorizontalFlipped; 
        [SerializeField]
        bool isVerticalFlipped;
        [Range(0,30)]
        public float decelSpeedDelta = 1; 
        [Range(0,30)]
        public float deltaMult = 1;
        [SerializeField]
        LayerMask layerMask;
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        AudioClip levelIntro;
        [SerializeField]
        AudioClip levelOutro;

        bool mouseMovedOnBackGround;

        public delegate void Notify();
        public static event Notify IntroCompleted;
        public static event Notify OutroCompleted;

        private void Awake() 
        {
            cam = GetComponent<Camera>();  
            cursor = GameObject.Instantiate(cursorGO,Vector3.one * 10000,Quaternion.identity).transform;
            cameraRotateDelta = new Vector2(0,0);
            cameraVelocity = new Vector2(0,0);
            previousMousePos = new Vector2(0,0);
            currentMousePos = new Vector2(0,0);

        }
        // Start is called before the first frame update
        
        public void Initialize(LevelData levelData,bool skipIntro)
        {
            isHorizontalFlipped = Global.Settings.Controls.lookHorizontalMode == MouseDragBeheaviour.Normal ? false : true;
            isVerticalFlipped = Global.Settings.Controls.lookVerticalMode == MouseDragBeheaviour.Normal ? false : true;
            zoomFactor = Global.Settings.Controls.scrollSensitivity;
            camSensitivity = Global.Settings.Controls.mouseDragSensitivity;
            minZoomDistance = levelData.minZoomDistance;
            cameraDistance = maxZoomDistance = levelData.maxZoomDistance;
            cam.transform.Rotate(new Vector3(1,0,0),40);
            cam.transform.Translate(new Vector3(0, 0, -maxZoomDistance));
            if(!skipIntro)
            {
                audioSource.clip = levelIntro;
                audioSource.Play();
                canControl = false;
                PlayLevelIntro();
            }
            else
            {
                canControl = true;
                IntroCompleted?.Invoke();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(canControl == false)
                return;
    
            CursorControl();
            currentMousePos = Input.mousePosition;
            mouseDelta = currentMousePos - previousMousePos;

            float mouseDeltaX = mouseDelta.x * camSensitivity;
            float mouseDeltaY = mouseDelta.y * camSensitivity;
            if(isHorizontalFlipped)
                mouseDeltaX *= -1; 
            if(isVerticalFlipped)
                mouseDeltaY *= -1; 
                
            mouseDelta = new Vector2(mouseDeltaX,mouseDeltaY);
                
            // Debug.Log(mouseDelta);
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            float stopMultiplier = 1;
            if(Physics.Raycast(ray, out hit, 750) && UnityEngine.Input.GetMouseButtonDown(0))
            {
                BubbleInput();
            }
            else
            {
                mouseMovedOnBackGround = true;
                if(Input.GetMouseButton(LeftClick))
                {
                    stopMultiplier = 5;
                    CameraUpdate();
                }
            }
            ScrollUpdate();
            previousMousePos = currentMousePos;
            mouseMovedOnBackGround = false; 

            cameraRotateDelta.x = Mathf.MoveTowards(cameraRotateDelta.x,0,Time.deltaTime * decelSpeedDelta * stopMultiplier);
            cameraRotateDelta.y = Mathf.MoveTowards(cameraRotateDelta.y,0,Time.deltaTime * decelSpeedDelta * stopMultiplier);
            // cameraRotateDelta.x = Mathf.Clamp(cameraRotateDelta.x + cameraVelocity.x * Time.deltaTime,-10,10);
            // cameraRotateDelta.y = Mathf.Clamp(cameraRotateDelta.y + cameraVelocity.y * Time.deltaTime,-10,10);

            RotateCamera();
        }
        void PlayLevelIntro()
        {
            FadeController.Fade(FadeController.FadeColor.White,FadeController.FadeColor.Clear,FadeController.FadeType.EaseOutQuart,5);
            StartCoroutine(LevelIntro());
        }
        IEnumerator LevelIntro()
        {
            float t = 0;
            Vector3 startingPos = cameraTarget.position = new Vector3(0,beginningHeight,0);
            Vector3 endPos = Vector3.zero;
            
            while (t < animHoverDownTime)
            {
                float finalPercent = Mathf.Clamp01(t / animHoverDownTime);
                float posCurvePercentage = EasingUtil.EaseOutQuad(finalPercent);
                float posRotPercentage = introRotationCurve.Evaluate(finalPercent);
                cameraTarget.position = Vector3.LerpUnclamped(startingPos,endPos, posCurvePercentage);

                float p = Mathf.Lerp(5,0,posRotPercentage);

                cam.transform.position = cameraTarget.position;
                cam.transform.Rotate(new Vector3(0,1,0),p, Space.World);
                // cam.transform.rotation = Quaternion.Euler(40,p,0);
                cam.transform.Translate(new Vector3(0, 0, -cameraDistance));
                yield return null;
                t += Time.deltaTime;
            }
            IntroCompleted?.Invoke();
            canControl = true;
            yield return null;
        }

        private void CameraUpdate()
        {
            if (mouseMovedOnBackGround)
            {
                cameraRotateDelta.x += (mouseDelta.x) * Time.deltaTime * deltaMult;
                cameraRotateDelta.y += (mouseDelta.y ) * Time.deltaTime * deltaMult;
            }

        }
        void ScrollUpdate()
        {
            cameraDistance = Mathf.Clamp(cameraDistance -= Input.mouseScrollDelta.y * zoomFactor,minZoomDistance,maxZoomDistance); 
            //TODO: Add sensitivity
        }

        void RotateCamera()
        {
            if(cam.transform.eulerAngles.x > 70)
            {
                cameraRotateDelta.y = -0.2f;
                cameraVelocity.y = 0;
            }
            else if(cam.transform.eulerAngles.x < 40)
            {
                cameraRotateDelta.y = 0.2f;
                cameraVelocity.y = 0;
            }

            cam.transform.position = cameraTarget.position;
            
            cam.transform.Rotate(new Vector3(0,1,0),cameraRotateDelta.x, Space.World);
            cam.transform.Rotate(new Vector3(1,0,0),cameraRotateDelta.y);

            cam.transform.Translate(new Vector3(0, 0, -cameraDistance));

            // float xRotation = Mathf.Clamp(cam.transform.position.y,0,360);
            // float yRotation = 0;
            // transform.eulerAngles = new Vector3 (xRotation,cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);
            

            // cameraTargetPos.transform.Translate(new Vector3(cameraTranslateDeltaX,-cameraTranslateDeltaY,0),camera.transform);
        }
        
        void BubbleInput()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 750,layerMask) && UnityEngine.Input.GetMouseButtonDown(0))
            {
                bubbleSpawn.SpawnBubble(hit.point);
            }
        }

        void CursorControl()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 750,layerMask))
            {
                Cursor.visible = false;
                cursor.position = hit.point;
            }
            else
            {
                Cursor.visible = true;
                cursor.position = Vector3.one * 10000;
            }
        }
        public void PlayOutroAnimation()
        {
            StartCoroutine(LevelOutro());
        }
        IEnumerator LevelOutro()
        {
            float maxTime = 6;
            canControl = false;
            float t = 0;
            Vector3 startingPos = Vector3.zero;
            Vector3 endPos = Vector3.up * 100;
            float newDistance = cameraDistance;
            bool hasPlayedFade = false;
            audioSource.clip = levelOutro;
            audioSource.Play();
            while (t < maxTime)
            {
                // newDistance += Time.deltaTime * 2;
                float finalPercent = Mathf.Clamp01(t / animHoverDownTime);
                float posCurvePercentage = EasingUtil.EaseInOutQuad(finalPercent);
                float posRotPercentage = introRotationCurve.Evaluate(finalPercent);
                cameraTarget.position = Vector3.LerpUnclamped(startingPos,endPos, posCurvePercentage);

                float p = Mathf.Lerp(0,10,posRotPercentage);
                float d = Mathf.Lerp(newDistance,newDistance * 3,posRotPercentage);

                cam.transform.position = cameraTarget.position;
                cam.transform.Rotate(new Vector3(0,1,0),p, Space.World);
                // cam.transform.rotation = Quaternion.Euler(40,p,0);
                cam.transform.Translate(new Vector3(0, 0, -d));
                if(hasPlayedFade == false && finalPercent > 0.2f)
                {
                    hasPlayedFade = true;
                    FadeController.Fade(FadeController.FadeColor.Clear,FadeController.FadeColor.White,FadeController.FadeType.EaseInOutCubic,4);
                }
                yield return null;
                t += Time.deltaTime;

            }
            OutroCompleted?.Invoke();
            yield return null;
        }
    }
}