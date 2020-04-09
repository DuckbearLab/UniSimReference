using EventReports;
using NetStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class EventReportEncoder
{
    public IntPtr EventReportPduPtr { get; private set; }

    public EntityId SenderId
    {
        get { return NetSimAgent.Instance.SenderId(EventReportPduPtr); }
        set { NetSimAgent.Instance.SetSenderId(EventReportPduPtr, value); }
    }

    public EntityId ReceiverId
    {
        get { return NetSimAgent.Instance.ReceiverId(EventReportPduPtr); }
        set { NetSimAgent.Instance.SetReceiverId(EventReportPduPtr, value); }
    }

    private int readingFixedIndex;
    private int readingVarIndex;
    private bool isWriting;
    private object target;

    public EventReportEncoder(uint eventType, object target)
    {
        isWriting = true;
        EventReportPduPtr = NetSimAgent.Instance.CreateEventReportInteraction(eventType);
        this.target = target;
    }

    public EventReportEncoder(IntPtr pduToReadPtr, object target)
    {
        isWriting = false;
        readingFixedIndex = readingVarIndex = 1;
        EventReportPduPtr = pduToReadPtr;
        this.target = target;
    }

    ~EventReportEncoder()
    {
        if (isWriting)
            NetSimAgent.Instance.DeleteEventReportInteraction(EventReportPduPtr);
    }

    public object Read(Type type, FieldInfo field = null)
    {
        if (type == typeof(int)) return ReadParamInt();
        if (type == typeof(bool)) return ReadParamBool();
        if (type == typeof(uint)) return ReadParamUInt();
        if (type == typeof(ushort)) return ReadParamUShort();
        if (type == typeof(float)) return ReadParamFloat();
        if (type == typeof(double)) return ReadParamDouble();
        if (type == typeof(string)) return ReadParamString();
        if (type == typeof(Vector3)) return ReadParamVector3();
        if (type == typeof(EntityId)) return ReadParamEntityId();
        if (type == typeof(EntityType)) return ReadParamEntityType();
        if (type == typeof(EventID)) return ReadParamEventID();
        if (type.IsEnum) return ReadParamInt(); //implicit conversion from int to enum, might break.
        if (type.IsArray) return ReadArray(field);
        if (type.IsValueType && !type.IsPrimitive) return ReadStruct(type);

        throw new Exception("Unknown type to read on field " + field.Name + "!");
    }

    public void Write(object value, FieldInfo field = null)
    {
        var valueType = field != null ? field.FieldType : value.GetType();
        if (valueType == typeof(int)) WriteParam((int)value, 0, field);
        else if (valueType == typeof(uint)) WriteParam((uint)value);
        else if (valueType == typeof(bool)) WriteParam((bool)value);
        else if (valueType == typeof(ushort)) WriteParam((ushort)value);
        else if (valueType == typeof(float)) WriteParam((float)value);
        else if (valueType == typeof(double)) WriteParam((double)value);
        else if (valueType == typeof(string)) WriteParam((string)value);
        else if (valueType == typeof(Vector3)) WriteParam((Vector3)value);
        else if (valueType == typeof(EntityId)) WriteParam((EntityId)value);
        else if (valueType == typeof(EntityType)) WriteParam((EntityType)value);
        else if (valueType == typeof(EventID)) WriteParam((EventID)value);
        else if (valueType.IsEnum) WriteParam((int)value);
        else if (valueType.IsArray) WriteParam((Array)value, field);
        else if (valueType.IsValueType && !valueType.IsPrimitive) WriteStruct(value);
        else throw new Exception("Unknown type to write on field " + field.Name + "!");
    }

    #region int
    private void WriteParam(int data, uint datumId = 0, FieldInfo field = null)
    {
        if (field != null && field.GetCustomAttributes(typeof(EventReportCalculatedAttribute), true).Length > 0)
        {
            foreach (var targetField in target.GetType().GetFields())
            {
                foreach (EventReportArrayAttribute attribute in targetField.GetCustomAttributes(typeof(EventReportArrayAttribute), true))
                {
                    if (attribute.ArrayLengthField == field.Name)
                    {
                        Array arr = (Array)targetField.GetValue(target);
                        if(arr != null)
                            NetSimAgent.Instance.AddFixedInt(EventReportPduPtr, arr.Length, datumId);
                        else
                            NetSimAgent.Instance.AddFixedInt(EventReportPduPtr, 0, datumId);
                        return;
                    }
                }
            }
            throw new Exception("Could not calculate field " + field.Name + "!");
        }
        else
        {
            NetSimAgent.Instance.AddFixedInt(EventReportPduPtr, data, datumId);
        }
    }

    private int ReadParamInt()
    {
        return NetSimAgent.Instance.ReadFixedInt(EventReportPduPtr, readingFixedIndex++);
    }
    #endregion

    #region bool
    private void WriteParam(bool data, uint datumId = 0)
    {
        NetSimAgent.Instance.AddFixedInt(EventReportPduPtr, data ? 1 : 0, datumId);
    }

    private bool ReadParamBool()
    {
        return NetSimAgent.Instance.ReadFixedInt(EventReportPduPtr, readingFixedIndex++) % 2 == 1;
    }
    #endregion

    #region uint
    private void WriteParam(uint data, uint datumId = 0)
    {
        NetSimAgent.Instance.AddFixedUInt(EventReportPduPtr, data, datumId);
    }

    private uint ReadParamUInt()
    {
        return NetSimAgent.Instance.ReadFixedUInt(EventReportPduPtr, readingFixedIndex++);
    }
    #endregion

    #region ushort
    private void WriteParam(ushort data, uint datumId = 0)
    {
        NetSimAgent.Instance.AddFixedUInt(EventReportPduPtr, data, datumId);
    }

    private ushort ReadParamUShort()
    {
        return (ushort)NetSimAgent.Instance.ReadFixedUInt(EventReportPduPtr, readingFixedIndex++);
    }
    #endregion

    #region float
    private void WriteParam(float data, uint datumId = 0)
    {
        NetSimAgent.Instance.AddFixedFloat(EventReportPduPtr, data, datumId);
    }

    private float ReadParamFloat()
    {
        return NetSimAgent.Instance.ReadFixedFloat(EventReportPduPtr, readingFixedIndex++);
    }
    #endregion

    #region double
    //loss of data, but the same as c++ BlDataStruct
    private void WriteParam(double data, uint datumId = 0)
    {
        NetSimAgent.Instance.AddFixedFloat(EventReportPduPtr, (float)data, datumId);
    }

    private double ReadParamDouble()
    {
        return (double)NetSimAgent.Instance.ReadFixedFloat(EventReportPduPtr, readingFixedIndex++);
    }
    #endregion

    #region string
    private void WriteParam(string data, uint datumId = 0)
    {
        if (data != null)
        {
            int dataLength = Encoding.UTF8.GetByteCount(data);

            NetSimAgent.Instance.AddVarString(EventReportPduPtr, data, dataLength, datumId);
        }
        else
            NetSimAgent.Instance.AddVarString(EventReportPduPtr, "", 0, datumId);
    }

    private string ReadParamString()
    {
        IntPtr charPtr = NetSimAgent.Instance.ReadVarString(EventReportPduPtr, readingVarIndex++);
        return Marshal.PtrToStringAnsi(charPtr);
    }
    #endregion

    #region Vector3
    private void WriteParam(Vector3 vec)
    {
        WriteParam(vec.x);
        WriteParam(vec.y);
        WriteParam(vec.z);
    }

    private Vector3 ReadParamVector3()
    {
        return new Vector3()
        {
            x = (float)ReadParamDouble(),
            y = (float)ReadParamDouble(),
            z = (float)ReadParamDouble()
        };
    }
    #endregion

    #region EntityId
    private void WriteParam(EntityId entityId)
    {
        WriteParam(entityId.site, 15100);
        WriteParam(entityId.host, 15200);
        WriteParam(entityId.entity, 15300);
    }

    private EntityId ReadParamEntityId()
    {
        return new EntityId()
        {
            site = ReadParamInt(),
            host = ReadParamInt(),
            entity = ReadParamUShort()
        };
    }
    #endregion

    #region EntityType
    private void WriteParam(EntityType entityType)
    {
        WriteParam(entityType.entityKind, 11110);
        WriteParam(entityType.domain, 11120);
        WriteParam(entityType.country, 11130);
        WriteParam(entityType.category, 11140);
        WriteParam(entityType.subCategory, 11150);
        WriteParam(entityType.specific, 11160);
        WriteParam(entityType.extra, 11170);
    }

    private EntityType ReadParamEntityType()
    {
        return new EntityType()
        {
            entityKind = ReadParamInt(),
            domain = ReadParamInt(),
            country = ReadParamInt(),
            category = ReadParamInt(),
            subCategory = ReadParamInt(),
            specific = ReadParamInt(),
            extra = ReadParamInt()
        };
    }
    #endregion

    #region EventID
    private void WriteParam(EventID entityId)
    {
        WriteParam(entityId.site, 500000);
        WriteParam(entityId.host, 500010);
        WriteParam(entityId.eventNum, 500020);
    }

    private EventID ReadParamEventID()
    {
        return new EventID()
        {
            site = ReadParamInt(),
            host = ReadParamInt(),
            eventNum = ReadParamInt()
        };
    }
    #endregion

    #region Array
    private void WriteParam(Array array, FieldInfo field)
    {
        if(array != null)
            foreach (var item in array)
                Write(item);
    }

    private Array ReadArray(FieldInfo field)
    {
        var arrayLengthAttributes = field.GetCustomAttributes(typeof(EventReportArrayAttribute), true);
        if (arrayLengthAttributes.Length > 0)
        {
            var arrayLengthAttribute = (EventReportArrayAttribute)arrayLengthAttributes[0];
            var arrayLengh = (int)target.GetType().GetField(arrayLengthAttribute.ArrayLengthField).GetValue(target);
            var arrayElementType = field.FieldType.GetElementType();

            if (arrayLengh < 0) // Is this the right thing to do?
                arrayLengh = 0;

            var array = Array.CreateInstance(arrayElementType, arrayLengh);

            for (int i = 0; i < arrayLengh; i++)
                array.SetValue(Read(arrayElementType), i);

            return array;
        }
        else
        {
            Debug.Log("EventReportArray Attribute not found on field" + field.Name + "!");
        }
        return new int[] { 1, 2, 3 };
    }
    #endregion

    #region Struct
    private void WriteStruct(object obj)
    {
        var prevTarget = target;

        target = obj;

        foreach (var field in obj.GetType().GetFields())
        {
            var value = field.GetValue(obj);
            Write(value, field);
        }

        target = prevTarget;
    }

    private object ReadStruct(Type fieldType)
    {
        var prevTarget = target;

        object o = Activator.CreateInstance(fieldType);

        target = o;

        foreach (var field in fieldType.GetFields())
        {
            field.SetValue(o, Read(field.FieldType, field));
        }

        target = prevTarget;

        return o;
    }
    #endregion

}