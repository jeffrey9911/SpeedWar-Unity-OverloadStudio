using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Publisher : MonoBehaviour
{
    List<Observer> observers = new List<Observer>();

    public void Notify()
    {
        for(int i = 0; i < observers.Count; i++)
        {
            observers[i].OnNotified();
        }
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }
}
