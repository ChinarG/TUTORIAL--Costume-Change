using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// 控制按钮Toggle脚本
/// </summary>
public class ChangePerson : MonoBehaviour
{
    private Toggle MyToggle; //自身的Toggle组件


    /// <summary>
    /// 初始化方法
    /// </summary>
    void Start()
    {
        if (gameObject.name == "SaveButton") //如果按钮名为：SaveButton
        {
            Button but = GetComponent<Button>();
            but.onClick.AddListener(LoadScene); //给当前物体上的Button组件添加监听事件
            return;                             //不在向下执行
        }

        MyToggle = GetComponent<Toggle>();
        MyToggle.onValueChanged.AddListener(OnValueChangePerson); //绑定Toggle事件
        //MyToggle.onValueChanged.AddListener((bool value) => OnValueChangePerson(value)); //lambda表达式转换为委托类型                                                            
    }


    /// <summary>
    /// Toggle方法，改变
    /// </summary>
    /// <param name="isChange"></param>
    public void OnValueChangePerson(bool isChange)
    {
        if (isChange)
        {
            if (gameObject.name == "Boy" || gameObject.name == "Girl")
            {
                MainChange.Instance.ChanggeSex(); //改变性别
                return;
            }

            string[] nameStrings = gameObject.name.Split('-');
            MainChange.Instance.ChangePersonPart(nameStrings[0], nameStrings[1]);
            switch (nameStrings[0])
            {
                case "Top":
                    PlayAnimation("item_shirt");
                    break;
                case "Pant":
                    PlayAnimation("item_pants");
                    break;
                case "Shoe":
                    PlayAnimation("item_boots");
                    break;
            }
        }
    }


    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="aniName"></param>
    private void PlayAnimation(string aniName)
    {
        Animation animation = GameObject.FindWithTag("Player").GetComponent<Animation>();
        if (!animation.IsPlaying(aniName)) //动画机是否在正在播放（aniName）这个动画，取反：意思是没有播放
        {
            animation.Play(aniName);       //那就，播放指定名字的动画
            animation.PlayQueued("idle1"); //播放完上一个，继续播放默认动画
        }
    }


    /// <summary>
    /// 加载其他场景
    /// </summary>
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}