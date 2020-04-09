using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public interface ITickable
    {
        void Tick();
    }

    private static Ticker _instance;
    private static Ticker Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = (GameObject.Find("Globals") ?? new GameObject("Ticker")).AddComponent<Ticker>();
            }
            return _instance;
        }
    }

    private List<ITickable> toTick = new List<ITickable>();
    private Queue<ITickable> toAdd = new Queue<ITickable>(), toRemove = new Queue<ITickable>();

    void Awake()
    {
        _instance = this;
    }

    public static void Subscribe(ITickable tickable)
    {
        Instance.toAdd.Enqueue(tickable);
    }

    public static void Unsubscribe(ITickable tickable)
    {

        Instance.toRemove.Enqueue(tickable);
    }

    // Update is called once per frame
    void Update()
    {
        //this allows safe removal from the tick function, which would otherwise bug the foreach over toTick
        //since the collection might change while ticking
        while (toAdd.Count > 0)
            toTick.Add(toAdd.Dequeue());
        while (toRemove.Count > 0)
            toTick.Remove(toRemove.Dequeue());
        foreach (var tickable in toTick)
            tickable.Tick();
    }
}
