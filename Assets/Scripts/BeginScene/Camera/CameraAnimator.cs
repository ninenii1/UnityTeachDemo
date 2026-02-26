using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; //使用委托需要引用的命名空间

public class CameraAnimator : MonoBehaviour
{
    private Animator animator;

    //用于记录动画播放完成过后，想要做的事情的函数
    private UnityAction overAction; //声明一个委托
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    //左转
    public void TurnLeft(UnityAction action)
    {
        animator.SetTrigger("Left");
        overAction = action;
    }

    //右转
    public void TurnRgiht(UnityAction action)
    {
        animator.SetTrigger("Right");
        overAction = action;
    }

    //当动画播放完时 会调用的方法
    public void PlayerOver()
    {
        overAction?.Invoke();
        overAction = null;
    }

}
