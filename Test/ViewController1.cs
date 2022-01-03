using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public enum ViewType
    {
        ThirdPersonBack, ThirdPersonFront, FirstPerson
    }
    public class ViewController1 : MonoBehaviour
    {
        public Camera thirdPersonCam;
        public Vector3 thirdPersonBackCamDir;//第三人称默认相机的方向（相对人物）
        public Vector3 thirdPersonFrontCamPos;//第三人称前置相机的位置
        public Transform lookAt;//注视位置
        public float rotateSpeed;//视角转向速度
        public float lookDownThreold;//向下看的最大角度
        public float lookUpThreshold;//向上看的最大角度
        public float lookAroundRange;//向左右看的最大角度
        public float followDistance;//标准跟随距离
        public float maxFollowDistance;//最大跟随距离
        public float followSpeed;//移动时相机后拉的速度
        public float resumeSpeed;//静止时相机恢复的速度
        public ViewType ViewType
        {
            get => viewtype;
        }
        private ViewType viewtype;

        private void Init()
        {
            if(!thirdPersonCam) thirdPersonCam = Camera.main;
        }
        void Awake()
        {
            Init();
        }
        void Start()
        {
            viewtype = ViewType.ThirdPersonBack;
            thirdPersonCam.transform.SetParent(transform);
            SetCamTransform(viewtype);
        }
        void Update()
        {

        }
        //如果不在平面上移动的话可以考虑获取人物的移动方向来作为摄像机的移动方向
        void FixedUpdate()
        {
            if(viewtype == ViewType.ThirdPersonBack)
            {
                UpdateCamPos();
                UpdateCamRot();
            }
            if(Input.GetKeyDown(KeyCode.V)) ChangeViewAspect();
        }
        void UpdateCamPos()
        {
            //模拟相机延迟跟随的效果
            //注意这里使用的都是相对位置，这样不会致使在人物收到外力移动时产生bug，所以followDistance和maxDistance也都应该设置为相对位置
            //followSpeed和resumeSpeed决定了在奔跑时和静止时相机推进的速度
            //从当前位置到最大/最小位置进行插值
            float currentDistance = Vector3.Magnitude(thirdPersonCam.transform.localPosition);
            float distance;
            if(Input.GetKey(KeyCode.W))
            {
                distance = Mathf.Lerp(currentDistance, maxFollowDistance, Time.deltaTime * followSpeed);
            }
            else
            {
                distance = Mathf.Lerp(currentDistance, followDistance, Time.deltaTime * resumeSpeed);
            }
            thirdPersonCam.transform.localPosition = thirdPersonBackCamDir.normalized * distance;        
        }
        void UpdateCamRot()
        {
            //左右旋转视角
            float rotateX = -Input.GetAxis("Mouse Y");
            float rotateY = Input.GetAxis("Mouse X");
            //使用直接设置localEulerAngles或者先自转x再公转y的方式避免欧拉旋转带来的绕z轴旋转问题
            //注意获取到的角的范围是在0-360，想实现中心对称clamp需要进行平移
            float currentX, currentY;
            if(thirdPersonCam.transform.localEulerAngles.x >= 180) currentX = thirdPersonCam.transform.localEulerAngles.x - 360;
            else currentX = thirdPersonCam.transform.localEulerAngles.x;
            if(thirdPersonCam.transform.localEulerAngles.y >= 180) currentY = thirdPersonCam.transform.localEulerAngles.y - 360;
            else currentY = thirdPersonCam.transform.localEulerAngles.y;
            float angX = Mathf.Clamp(currentX + rotateX * rotateSpeed * Time.deltaTime, lookDownThreold, lookUpThreshold);
            float angY = Mathf.Clamp(currentY + rotateY * rotateSpeed * Time.deltaTime, -lookAroundRange, lookAroundRange);
            thirdPersonCam.transform.localEulerAngles = new Vector3(angX, angY, 0);
            //thirdPersonCam.transform.Rotate(x, 0, 0, Space.Self);
            //thirdPersonCam.transform.Rotate(0, y, 0, Space.World);
        }
        void ChangeViewAspect()
        {
            switch(viewtype)
            {
                case ViewType.ThirdPersonBack:
                    viewtype = ViewType.ThirdPersonFront;
                break;
                case ViewType.ThirdPersonFront:
                    viewtype = ViewType.ThirdPersonBack;
                break;
            }
            SetCamTransform(viewtype);
        }
        void SetCamTransform(ViewType vt)
        {
            switch(viewtype)
            {
                case ViewType.ThirdPersonBack:
                    thirdPersonCam.transform.localPosition = thirdPersonBackCamDir.normalized * followDistance;
                break;
                case ViewType.ThirdPersonFront:
                    thirdPersonCam.transform.localPosition = thirdPersonFrontCamPos;
                break;
            }
            thirdPersonCam.transform.LookAt(lookAt);
        }
    }
}
