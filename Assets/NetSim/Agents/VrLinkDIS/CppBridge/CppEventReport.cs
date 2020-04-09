using NetStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class CppEventReport
{
    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateEventReportInteraction(uint eventType);

    [DllImport("VrLinkDIS")]
    public extern static void DeleteEventReportInteraction(IntPtr pdu);

    [DllImport("VrLinkDIS")]
    public extern static void SetSenderId(IntPtr pdu, EntityId senderId);

    [DllImport("VrLinkDIS")]
    public extern static EntityId SenderId(IntPtr pdu);

    [DllImport("VrLinkDIS")]
    public extern static void SetReceiverId(IntPtr pdu, EntityId recieverId);
    
    [DllImport("VrLinkDIS")]
    public extern static EntityId ReceiverId(IntPtr pdu);

    [DllImport("VrLinkDIS")]
    public extern static void AddFixedInt(IntPtr eventReport, int data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static int ReadFixedInt(IntPtr eventReport, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddFixedUInt(IntPtr eventReport, uint data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static uint ReadFixedUInt(IntPtr eventReport, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddFixedFloat(IntPtr eventReport, float data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static float ReadFixedFloat(IntPtr eventReport, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddVarString(IntPtr eventReport, string data, int dataLength, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static IntPtr ReadVarString(IntPtr eventReport, int index);

    [DllImport("VrLinkDIS")]
    public extern static void SendEventReportInteraction(IntPtr exConn, IntPtr eventReport);

    [DllImport("VrLinkDIS")]
    public extern static void SetEventReportReceivedCallback(IntPtr exConnPtr, NetSimAgent.EventReportReceivedCallback eventReportReceivedCallback);

}
