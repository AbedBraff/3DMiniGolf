using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Generic stack class
 * Zachary Mitchell - 2/17/2019
 */

public class Stack<T>
{
    public Stack()
    {
        m_NumObjects = 0;
        m_Data = new T[1];
    }

    private int m_NumObjects;
    public T[] m_Data;


    //  Return number of current objects (not array length)
    public int GetNumObjects()
    {
        return m_NumObjects;
    }


    //  Add an object to the top of the stack
    public void Push(T _object)
    {
        if (m_NumObjects == m_Data.Length)
        {
            IncreaseSizeAndCopy();
        }

         AddAt(_object, m_NumObjects);
    }


    //  Remove an object from the top of the stack
    public void Pop()
    {
        if (m_NumObjects > 0)
        {
            RemoveAt(m_NumObjects - 1);
        }
        else
            Debug.Log("There are no items to remove.");
    }


    //  Add an object at the passed index value
    public void AddAt(T _object, int _index)
    {
        if (_index < 0 || _index > m_NumObjects)
        {
            Debug.Log("Out of bounds exception.");
            return;
        }

        if (m_NumObjects == m_Data.Length)
        {
            IncreaseSizeAndCopy();
        }

        if (_index == m_NumObjects)
            m_Data[m_NumObjects] = _object;
        else
        {
            for(int i = m_NumObjects; i >= _index; --i)
            {
                if(i == _index)
                    m_Data[i] = _object;
                else
                    m_Data[i] = m_Data[i - 1];
            }
        }

        m_NumObjects++;
    }


    //  Remove the object at the passed index value
    public void RemoveAt(int _index)
    {
        if (_index < 0 || _index > m_NumObjects)
        {
            Debug.Log("Out of bounds exception.");
            return;
        }

        m_Data[_index] = default(T);

        for(int i = _index; i < m_NumObjects - 1; ++i)
        {
            m_Data[i] = m_Data[i + 1];
        }


        System.GC.Collect();
        m_NumObjects--;
    }


    //  Return the object at this index
    public T GetObjectAt(int _index)
    {
        if(IsEmpty())
        {
            Debug.Log("There are no objects in this data structure.");
            return default(T);
        }
        if(_index < 0 || _index > (m_NumObjects - 1))
        {
            Debug.Log("Out of bounds exception. No object at " + _index + " exists.");
            return default(T);
        }

        return m_Data[_index];
    }


    //  Clear the data structure
    public void Clear()
    {
        m_Data = new T[1];
        System.GC.Collect();
        m_NumObjects = 0;
    }



    public bool IsEmpty()
    {
        return m_NumObjects == 0;
    }



    //  Increase size of the array when needed
    private void IncreaseSizeAndCopy()
    {
        T[] temp = new T[m_NumObjects * 2];

        for (int i = 0; i < m_NumObjects; ++i)
        {
            temp[i] = m_Data[i];
        }

        m_Data = temp;
        System.GC.Collect();
    }


    //  Print data to the console
    public void PrintData()
    {
        if (IsEmpty())
            Debug.Log("Stack is empty.");

        for(int i = 0; i < m_NumObjects; ++i)
        {
            Debug.Log(m_Data[i].ToString() + ", ");
        }
    }
}
