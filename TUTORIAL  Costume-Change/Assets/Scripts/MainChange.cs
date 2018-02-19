using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 换装脚本
/// </summary>
public class MainChange : MonoBehaviour
{
    public static MainChange  Instance { get; set; } //单例
    private       Transform   GirlSourceTrans;       //女孩资源预设物位置
    private       GameObject  GirlTarget;            //需要被换装的女孩
    private       Transform[] GirlHips;              //空骨架女孩骨骼信息
    private       Transform   BoySourceTrans;        //男孩资源预设物位置
    private       GameObject  BoyTarget;             //需要被换装的男孩
    private       Transform[] BoyHips;               //空骨架男孩骨骼信息
    public        int         SexCount = 0;          //性别标示
    public        GameObject  BoyPanel;              //男孩换装页面
    public        GameObject  GirlPanel;             //女孩换装页面

    private string[,]                                                   GirlStr  = new string[,] {{"Eye", "1"}, {"Hair", "1"}, {"Top", "1"}, {"Pant", "1"}, {"Shoe", "1"}, {"Face", "1"}}; //二维数组，用来存默认套装
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> GirlData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();                                      //新建一个字典，用来存预设物中的 每个部位的名字，编号，对应skin
    private Dictionary<string, SkinnedMeshRenderer>                     GirlSmr  = new Dictionary<string, SkinnedMeshRenderer>();                                                          //新建一个自身的字典，换装骨骼身上的Skin的信息
    private string[,]                                                   BoyStr   = new string[,] {{"Eye", "1"}, {"Hair", "1"}, {"Top", "1"}, {"Pant", "1"}, {"Shoe", "1"}, {"Face", "1"}};
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> BoyData  = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
    private Dictionary<string, SkinnedMeshRenderer>                     BoySmr   = new Dictionary<string, SkinnedMeshRenderer>(); //换装骨骼身上的Skin的信息


    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); //场景切换不删除
    }


    /// <summary>
    /// 初始化函数
    /// </summary>
    void Start()
    {
        GirlClone();                            //调用女孩所有资源
        BoyClone();                             //调用男孩
        BoyTarget.AddComponent<FollowMouse>();  //在物体上添加鼠标可控脚本
        GirlTarget.AddComponent<FollowMouse>(); //同理
        BoyTarget.SetActive(false);             //默认男孩加载后，失活
    }


    /// <summary>
    /// 女孩资源全部加载
    /// </summary>
    public GameObject GirlClone()
    {
        InstantiateGirlAsset();
        SaveData(GirlSourceTrans, GirlData, GirlTarget, GirlSmr);
        InitGirl();
        return GirlTarget; //便于其他场景处理，所以返回一个GameObject对象
    }


    /// <summary>
    /// 男孩资源全部加载
    /// </summary>
    public GameObject BoyClone()
    {
        InstantiateBoyAsset();
        SaveData(BoySourceTrans, BoyData, BoyTarget, BoySmr);
        InitBoy();
        return BoyTarget; //便于其他场景处理，所以返回一个GameObject对象
    }


    /// <summary>
    /// 初始化女孩资源
    /// </summary>
    private void InstantiateGirlAsset()
    {
        GameObject GirlPrefab = Instantiate(Resources.Load<GameObject>("FemaleModel")); //加载女性骨骼预设物
        GirlSourceTrans       = GirlPrefab.transform;                                   //记录女孩被实例化后的位置信息
        GirlPrefab.SetActive(false);                                                    //关闭
        GirlTarget = (GameObject) Instantiate(Resources.Load("FemaleTest"));            //加载女性预设物，赋值给
        GirlHips   = GirlTarget.GetComponentsInChildren<Transform>();                   //获取骨骼信息，存入GirlHips
    }


    /// <summary>
    /// 初始化男孩资源
    /// </summary>
    private void InstantiateBoyAsset()
    {
        GameObject BoyPrefab = Instantiate(Resources.Load<GameObject>("MaleModel")); //加载男性骨骼预设物
        BoySourceTrans       = BoyPrefab.transform;                                  //记录男性被实例化后的位置信息
        BoyPrefab.SetActive(false);                                                  //关闭
        BoyTarget = (GameObject) Instantiate(Resources.Load("MaleTest"));            //加载男性预设物，赋值给
        BoyHips   = BoyTarget.GetComponentsInChildren<Transform>();                  //获取骨骼信息，存入BoyHips
    }


    /// <summary>
    /// 保存数据
    /// </summary>
    private void SaveData(Transform sourceTransform, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, GameObject target, Dictionary<string, SkinnedMeshRenderer> mydic)
    {
        data.Clear();                //清空大字典
        mydic.Clear();               //清空自身骨架小字典
        if (sourceTransform == null) //容错
        {
            return;
        }

        SkinnedMeshRenderer[] parts = sourceTransform.GetComponentsInChildren<SkinnedMeshRenderer>(); //获取所有带有siki的物体，存入parts
        foreach (var part in parts)//遍历蒙皮网格数组中的每一个元素
        {
            string[] nameStrings = part.name.Split('-'); //记录名字，用-拆分，输出的值为:    “Test-1”----对应数组中------0:"Test"   1:"1"
            if (!data.ContainsKey(nameStrings[0]))       //如果字典GirlData中不包含 第一个资源预设下的子物体的名字
            {
                GameObject partObj       = new GameObject(); //实例化一个空物体
                partObj.name             = nameStrings[0];   //给物体改名
                partObj.transform.parent = target.transform; //设置父物体

                mydic.Add(nameStrings[0], partObj.AddComponent<SkinnedMeshRenderer>());  //把骨骼target身上的骨骼信息存起来
                data.Add(nameStrings[0], new Dictionary<string, SkinnedMeshRenderer>()); //存到字典中
            }

            data[nameStrings[0]].Add(nameStrings[1], part); //存储所有的skin信息到字典
        }
    }


    /// <summary>
    /// 改变mesh，传入部位和编号即可
    /// </summary>
    /// <param name="part"></param>
    /// <param name="num"></param>
    private void ChangeMesh(string part, string num, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, Transform[] hipsTransforms, Dictionary<string, SkinnedMeshRenderer> mydic, string[,] strtwo)
    {
        SkinnedMeshRenderer skin     = data[part][num];       //新建一个skin，并赋值：从字典中
        List<Transform>     boneList = new List<Transform>(); //新建一个列表

        foreach (var trans in skin.bones) //skin.bones能查到对应多少骨骼，返回值为：Transform[]
        {
            foreach (var girlHip in hipsTransforms)
            {
                if (girlHip.name == trans.name)
                {
                    boneList.Add(girlHip);
                    break; //添加骨骼
                }
            }
        }

        //下面是：换装
        mydic[part].bones      = boneList.ToArray(); //列表转数组,存入骨骼信息
        mydic[part].materials  = skin.materials;     //存入材质
        mydic[part].sharedMesh = skin.sharedMesh;    //存入mesh

        ChangeData(part, num, strtwo);
    }


    /// <summary>
    /// 初始化身体
    /// </summary>
    private void InitGirl()
    {
        int length = GirlStr.GetLength(0); //获得行数

        for (int i = 0; i < length; i++) //遍历行
        {
            ChangeMesh(GirlStr[i, 0], GirlStr[i, 1], GirlData, GirlHips, GirlSmr, GirlStr); //调用方法，传入值：穿上衣服（穿什么，编号）
        }
    }


    /// <summary>
    /// 初始化身体
    /// </summary>
    private void InitBoy()
    {
        int length = BoyStr.GetLength(0); //获得行数
        for (int i = 0; i < length; i++)  //遍历行
        {
            ChangeMesh(BoyStr[i, 0], BoyStr[i, 1], BoyData, BoyHips, BoySmr, BoyStr); //调用方法，传入值：穿上衣服（穿什么，编号）
        }
    }


    /// <summary>
    /// 改变人物部位
    /// </summary>
    public void ChangePersonPart(string part, string num)
    {
        if (SexCount == 0)
        {
            ChangeMesh(part, num, GirlData, GirlHips, GirlSmr, GirlStr); //改变小女孩
        }
        else
        {
            ChangeMesh(part, num, BoyData, BoyHips, BoySmr, BoyStr); //调用方法，传入值：穿上衣服（穿什么，编号）
        }
    }


    /// <summary>
    /// 改变性别
    /// </summary>
    public void ChanggeSex()
    {
        if (SexCount == 0)
        {
            SexCount = 1;
            BoyTarget.SetActive(true);
            BoyPanel.SetActive(true);
            GirlTarget.SetActive(false);
            GirlPanel.SetActive(false);
        }
        else
        {
            SexCount = 0;
            BoyTarget.SetActive(false);
            BoyPanel.SetActive(false);
            GirlTarget.SetActive(true);
            GirlPanel.SetActive(true);
        }
    }


    /// <summary>
    /// 更改数据,保存换过的衣服信息
    /// </summary>
    private void ChangeData(string part, string num, string[,] str)
    {
        int length = GirlStr.GetLength(0); //获得行数
        for (int i = 0; i < length; i++)
        {
            if (str[i, 0] == part)
            {
                str[i, 1] = num;
            }
        }
    }
}