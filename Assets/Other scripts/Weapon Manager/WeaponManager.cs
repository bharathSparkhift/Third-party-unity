using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponManager
{
    public abstract void Spawn_Weapon();
}

public class Rifle : WeaponManager
{

    public Stack<Rifle> stack = new Stack<Rifle>();

    public override void Spawn_Weapon()
    {
        Debug.Log($"{nameof(Rifle)} \t {nameof(Spawn_Weapon)}");
    }


    public virtual void Equip()
    {
        Debug.Log($"{nameof(Rifle)} \t {nameof(Equip)}");
    }

    public virtual void Drop()
    {
        Debug.Log($"{nameof(Rifle)} \t {nameof(Drop)}");
    }

    

}

public class AK47 : Rifle
{
    public override void Equip()
    {
        base.stack.Push( this );
        Debug.Log($"{nameof(Rifle)} \t {nameof(Equip)} \t Rifle count on stack {(base.stack.Count)}");
        
    }

    public override void Drop()
    {
        base.stack.Pop();
        if(base.stack.Count < 0)
            return;
        Debug.Log($"{nameof(Rifle)} \t {nameof(Equip)} \t Rifle count on stack {(base.stack.Count)}");
    }
}




