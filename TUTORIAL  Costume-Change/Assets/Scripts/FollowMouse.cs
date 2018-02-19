using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    private bool isClick;//是否点击鼠标
    private Vector3 NowPos;//当前坐标
    private Vector3 OldPos;//下一个坐标
    public float DefaulLength = 2;//默认长度

    /// <summary>
    /// 鼠标抬起
    /// </summary>
    private void OnMouseUp()
    {
        isClick = false;
    }

    /// <summary>
    /// 鼠标按下
    /// </summary>
    private void OnMouseDown()
    {
        isClick = true;
    }


	
	
	void Update ()
	{
	    NowPos = Input.mousePosition;
	    if (isClick)
	    {
	        Vector3 offsetVector3 = NowPos - OldPos;
            //如果 偏移量的X绝对值>大于偏移量Y，并且偏移量X>设置的默认长度
	        if (Mathf.Abs(offsetVector3.x)>Mathf.Abs(offsetVector3.y)&&Mathf.Abs(offsetVector3.x)>DefaulLength)
	        {
	            transform.Rotate(Vector3.up,-offsetVector3.x);//自身旋转，UP：沿着Y轴旋转
	        }
	    }

	    OldPos = Input.mousePosition;
	}
}
