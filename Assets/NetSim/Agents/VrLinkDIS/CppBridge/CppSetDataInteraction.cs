using NetStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class CppSetDataInteraction
{
    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateSetDataInteraction();

    [DllImport("VrLinkDIS")]
    public extern static void DeleteSetDataInteraction(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static EntityId SDISenderId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetSDISenderId(IntPtr interaction, EntityId senderId);

    [DllImport("VrLinkDIS")]
    public extern static EntityId SDIReceiverId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetSDIReceiverId(IntPtr interaction, EntityId recieverId);

    [DllImport("VrLinkDIS")]
    public extern static ulong SDIRequestId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetSDIRequestId(IntPtr interaction, ulong requestId);

    [DllImport("VrLinkDIS")]
    public extern static void AddSetDataInteractionFixedInt(IntPtr interaction, int data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static int ReadSetDataInteractionFixedInt(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddSetDataInteractionFixedUInt(IntPtr interaction, uint data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static uint ReadSetDataInteractionFixedUInt(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddSetDataInteractionFixedFloat(IntPtr interaction, float data, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static float ReadSetDataInteractionFixedFloat(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void AddSetDataInteractionVarString(IntPtr interaction, string data, int dataLength, uint datumParam);

    [DllImport("VrLinkDIS")]
    public extern static IntPtr ReadSetDataInteractionVarString(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int SetDataInteractionNumFixedFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int SetDataInteractionFixedDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int SetDataInteractionNumVarFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int SetDataInteractionVarDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static void SendSDI(IntPtr exConn, IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetSDIReceivedCallback(IntPtr exConnPtr, NetSimAgent.SDIReceivedCallback sDIReceivedCallback);

}
