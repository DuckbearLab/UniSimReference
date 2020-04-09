using NetStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class CppDataInteraction
{
    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateDataInteraction();

    [DllImport("VrLinkDIS")]
    public extern static void DeleteDataInteraction(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static EntityId DISenderId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDISenderId(IntPtr interaction, EntityId senderId);

    [DllImport("VrLinkDIS")]
    public extern static EntityId DIReceiverId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDIReceiverId(IntPtr interaction, EntityId recieverId);

    [DllImport("VrLinkDIS")]
    public extern static ulong DIRequestId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDIRequestId(IntPtr interaction, ulong requestId);

    [DllImport("VrLinkDIS")]
    public extern static void AddDataInteractionFixedInt(IntPtr interaction, int data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static int ReadDataInteractionFixedInt(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddDataInteractionFixedUInt(IntPtr interaction, uint data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static uint ReadDataInteractionFixedUInt(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddDataInteractionFixedFloat(IntPtr interaction, float data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static float ReadDataInteractionFixedFloat(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddDataInteractionVarString(IntPtr interaction, string data, int dataLength, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static IntPtr ReadDataInteractionVarString(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int DataInteractionNumFixedFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int DataInteractionFixedDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int DataInteractionNumVarFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int DataInteractionVarDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void SendDI(IntPtr exConn, IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDIReceivedCallback(IntPtr exConnPtr, NetSimAgent.DIReceivedCallback dIReceivedCallback);

}
