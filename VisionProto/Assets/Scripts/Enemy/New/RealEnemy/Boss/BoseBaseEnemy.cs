using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BaseEnemy;
using UnityEngine.ProBuilder;
using static BTNode;
using System;

public abstract class BoseBaseEnemy : MonoBehaviour, IListener
{
    protected BTNode baseBehaviorTree;
    protected BTNode baseBehaviorTree2;
    protected BTNode baseBehaviorTree3;
    protected Dictionary<BossActionType, BossAction> actions;

    public enum BossActionType
    {
        Idle,
        PazeOne,
        PazeOneClose,
        DistanceJudge,
        PazeTwo,
        PazeTwoClose,
        PazeThree,
        PazeThreeClose,
        MachineGun,
        Grab,
        Dash,
        DownStrike,
        Punch,
        Railgun,
        Invulnerable,
        DDash,
        DDownStrike,
        Grog,
        Die
    }

    public FloatWrapper HP = new FloatWrapper(600f);

    public Transform player;
    public NavMeshAgent agent;
    protected Animator animator;

    public Transform machinPivot;
    public Transform machinPivot2;

    public IntWrapper a = new IntWrapper(0);

    private bool isStop;
    public BoolWrapper grogExecuted = new BoolWrapper(false); // Grog 실행 여부를 체크하는 플래그
    public BoolWrapper grogExecuted2 = new BoolWrapper(false); // Grog 실행 여부를 체크하는 플래그
    public BoolWrapper grogEnd1 = new BoolWrapper(false); 
    public BoolWrapper grogEnd2 = new BoolWrapper(false);

    public bool x;
    public bool y;
    public bool z ;

    public GameObject[] particle;
    public GameObject deadScene;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.Find("Head").transform;
        }

        InitializeActions();
        InitializeBehaviorTree();
    }

    // Update is called once per frame
    protected void Update()
    {

        if (HP.Value <= 3000 && HP.Value > 1500)
        {
            grogExecuted.Value = true;
            particle[0].SetActive(true);
        }
        if (HP.Value <= 1500 && !grogExecuted2.Value)
        {
            grogExecuted2.Value = true;
            particle[1].SetActive(true);
            particle[2].SetActive(true);
            particle[3].SetActive(true);

        }


        if (isStop)
        {
            actions[BossActionType.PazeOne].Stop();
            actions[BossActionType.PazeTwo].Stop();
            actions[BossActionType.PazeThree].Stop();
            actions[BossActionType.Idle].Stop();
            return;
        }

        


        if (HP.Value <= 0)
        {
            actions[BossActionType.PazeOne].Stop();
            actions[BossActionType.PazeTwo].Stop();
            actions[BossActionType.PazeThree].Stop();
            actions[BossActionType.Die].Execute();
        }
        else if (grogExecuted.Value && !grogEnd1.Value || grogExecuted2.Value && !grogEnd2.Value)
         {
            //if (grogEnd.Value) return;

            actions[BossActionType.PazeOne].Stop();
            actions[BossActionType.PazeTwo].Stop();
            actions[BossActionType.PazeThree].Stop();
            actions[BossActionType.Grog].Execute();
        }
        else if (HP.Value > 0)
        {
            if (HP.Value > 3000 && HP.Value <= 4500)
            {
                NodeState result = baseBehaviorTree.Execute();
            }
            else if (HP.Value > 1500 && HP.Value <= 3000)
            {
                NodeState result = baseBehaviorTree2.Execute();
            }
            else
            {
                NodeState result = baseBehaviorTree3.Execute();
            }
        }
    }


    

    protected abstract void InitializeBehaviorTree();

    protected virtual void InitializeActions()
    {
        actions = new Dictionary<BossActionType, BossAction>
        {
           { BossActionType.Idle, new BIdlee(agent,player, animator) },
           { BossActionType.Grog, new BGrog(animator,HP,grogEnd1, grogEnd2) },
           { BossActionType.PazeOne, new BPazeOne(agent,player, animator,machinPivot, a, HP) },
           { BossActionType.PazeOneClose, new BPazeOneClose(agent,player, animator,a, HP) },
           { BossActionType.PazeTwo, new BPazeTwo(agent,player, animator,machinPivot2, HP) },
           { BossActionType.PazeTwoClose, new BPazeTwoClose(agent,player, animator, HP) },
           { BossActionType.PazeThree, new BPazeThree(agent,player, animator,machinPivot2, HP) },
           { BossActionType.PazeThreeClose, new BPazeThreeClose(agent,player, animator, HP) },
           //{ BossActionType.PazeThree, new BPazeOne() },
           //{ BossActionType.MachineGun, new BPazeOne() },
           //{ BossActionType.Grab, new BPazeOne() },
           //{ BossActionType.Dash, new BPazeOne() },
           //{ BossActionType.DownStrike, new BPazeOne() },
           //{ BossActionType.Punch, new BPazeOne() },
           //{ BossActionType.Railgun, new BPazeOne() },
           //{ BossActionType.Invulnerable, new BPazeOne() },
           //{ BossActionType.DDash, new BPazeOne() },
           //{ BossActionType.DDownStrike, new BPazeOne() },
           //{ BossActionType.Grog, new BPazeOne() },
           { BossActionType.Die, new BDie(animator, deadScene) },
        };
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.isPause:
                isStop = (bool)param;
                break;
        }
    }

}

[Serializable]
public class IntWrapper
{
    public int Value;

    public IntWrapper(int value)
    {
        Value = value;
    }
}