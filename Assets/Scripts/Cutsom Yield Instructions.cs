using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * CutsomYieldInstructions -
 * DESCRIPTION - A place for all your custom yield instructions. 
 * just inherit from CustomYieldInstuction and override public bool keepWaiting { get { } }
 * to wait until whenever and do whatever you want. 
 * =================================================================================== */

//DoForSeconds performs the action given every frame, then keeps waiting if [[Duration]] seconds haven't passed yet or continues the coroutine if they have
public class DoForSeconds : CustomYieldInstruction
{
    private Action Action;

    private float EndTime;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action();
            return Time.time < EndTime;
        }
    }

    public DoForSeconds(float Duration, Action Action = null)
    {
        this.Action = Action;
        EndTime = Time.time + Duration;
    }
}
public class DoForSeconds<T1> : CustomYieldInstruction
{
    private Action<T1> Action;

    private float EndTime;
    private T1 Arg1;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1);
            return Time.time < EndTime;
        }
    }

    public DoForSeconds(float Duration, Action<T1> Action, T1 Arg1)
    {
        this.Action = Action;
        EndTime = Time.time + Duration;
        this.Arg1 = Arg1;
    }
}
public class DoForSeconds<T1, T2> : CustomYieldInstruction
{
    private Action<T1, T2> Action;

    private float EndTime;
    private T1 Arg1;
    private T2 Arg2;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2);
            return Time.time < EndTime;
        }
    }

    public DoForSeconds(float Duration, Action<T1, T2> Action, T1 Arg1, T2 Arg2)
    {
        this.Action = Action;
        EndTime = Time.time + Duration;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
    }
}
public class DoForSeconds<T1, T2, T3> : CustomYieldInstruction
{
    private Action<T1, T2, T3> Action;

    private float EndTime;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3);
            return Time.time < EndTime;
        }
    }

    public DoForSeconds(float Duration, Action<T1, T2, T3> Action, T1 Arg1, T2 Arg2, T3 Arg3)
    {
        this.Action = Action;
        EndTime = Time.time + Duration;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
    }
}
public class DoForSeconds<T1, T2, T3, T4> : CustomYieldInstruction
{
    private Action<T1, T2, T3, T4> Action;

    private float EndTime;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;
    private T4 Arg4;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3, Arg4);
            return Time.time < EndTime;
        }
    }

    public DoForSeconds(float Duration, Action<T1, T2, T3, T4> Action, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4)
    {
        this.Action = Action;
        EndTime = Time.time + Duration;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
    }
}

//DoWhile performs the action given every frame, then keeps waiting if the predicate returns true or continues the coroutine when the predicate returns false 
public class DoWhile : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action Action;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action();
            return Predicate();
        }
    }

    public DoWhile(Func<bool> Predicate, Action Action = null)
    {
        this.Action = Action;
        this.Predicate = Predicate;
    }
}
public class DoWhile<T1> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1> Action;
    private T1 Arg1;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1);
            return Predicate();
        }
    }

    public DoWhile(Func<bool> Predicate, Action<T1> Action, T1 Arg1)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
    }
}
public class DoWhile<T1, T2> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2> Action;
    private T1 Arg1;
    private T2 Arg2;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2);
            return Predicate();
        }
    }

    public DoWhile(Func<bool> Predicate, Action<T1, T2> Action, T1 Arg1, T2 Arg2)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
    }
}
public class DoWhile<T1, T2, T3> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2, T3> Action;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3);
            return Predicate();
        }
    }

    public DoWhile(Func<bool> Predicate, Action<T1, T2, T3> Action, T1 Arg1, T2 Arg2, T3 Arg3)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
    }
}
public class DoWhile<T1, T2, T3, T4> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2, T3, T4> Action;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;
    private T4 Arg4;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3, Arg4);
            return Predicate();
        }
    }

    public DoWhile(Func<bool> Predicate, Action<T1, T2, T3, T4> Action, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
    }
}

//DoUntil is the same as DoWhile, but does the action every frame until the condition is met. 
public class DoUntil : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action Action;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action();
            return !Predicate();
        }
    }

    public DoUntil(Func<bool> Predicate, Action Action = null)
    {
        this.Action = Action;
        this.Predicate = Predicate;
    }
}
public class DoUntil<T1> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1> Action;
    private T1 Arg1;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1);
            return !Predicate();
        }
    }

    public DoUntil(Func<bool> Predicate, Action<T1> Action, T1 Arg1)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
    }
}
public class DoUntil<T1, T2> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2> Action;
    private T1 Arg1;
    private T2 Arg2;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2);
            return !Predicate();
        }
    }

    public DoUntil(Func<bool> Predicate, Action<T1, T2> Action, T1 Arg1, T2 Arg2)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
    }
}
public class DoUntil<T1, T2, T3> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2, T3> Action;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3);
            return !Predicate();
        }
    }

    public DoUntil(Func<bool> Predicate, Action<T1, T2, T3> Action, T1 Arg1, T2 Arg2, T3 Arg3)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
    }
}
public class DoUntil<T1, T2, T3, T4> : CustomYieldInstruction
{
    private Func<bool> Predicate;

    private Action<T1, T2, T3, T4> Action;
    private T1 Arg1;
    private T2 Arg2;
    private T3 Arg3;
    private T4 Arg4;

    public override bool keepWaiting
    {
        get
        {
            if (Action != null)
                Action(Arg1, Arg2, Arg3, Arg4);
            return !Predicate();
        }
    }

    public DoUntil(Func<bool> Predicate, Action<T1, T2, T3, T4> Action, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4)
    {
        this.Action = Action;
        this.Predicate = Predicate;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
    }
}

/// <summary>
/// Use:
/// WaitForEvent wait = new WaitForEvent();
/// Event += Wait.Continue;
/// yield return wait;
/// Event -= Wait.Continue;
/// </summary>
public class WaitForEvent : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return waiting;
        }
    }

    private bool waiting;

    public WaitForEvent()
    {
        waiting = true;
    }

    public void Continue()
    {
        waiting = false;
    }

    public void Continue<T>(T eventParam)
    {
        waiting = false;
    }

    public void Continue<T1, T2>(T1 eventParam1, T2 eventParam2)
    {
        waiting = false;
    }

    public void Continue<T1, T2, T3>(T1 eventParam1, T2 eventParam2, T3 eventParam3)
    {
        waiting = false;
    }

    public new void Reset()
    {
        waiting = true;
        (this as CustomYieldInstruction).Reset();
    }
}