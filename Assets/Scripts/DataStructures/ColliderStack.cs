using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Zachary Mitchell - 5/1/2019
 * Extension of generic stack class into a stack of Colliders
 * Adds function to search the stack for a collider tag
 */

public class ColliderStack : Stack<Collider>
{
   public ColliderStack()
    {

    }


    //  Determine if there is a collider within the stack that has a matching tag to the value passed in
    public bool IsTagInStack(string _tag)
    {
        for (int i = 0; i < this.GetNumObjects(); ++i)
        {
            Collider col = this.GetObjectAt(i);
            if (col.CompareTag(_tag))
            {
                return true;
            }
        }

        return false;
    }


    public override string ToString()
    {
        string values = "";

        for(int i = 0; i < this.GetNumObjects(); ++i)
        {
            values += this.GetObjectAt(i).tag;

            if (i != this.GetNumObjects() - 1)
                values += ", ";
        }

        return values;
    }
}
