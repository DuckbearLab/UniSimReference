using NetStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class CppDataQueryInteraction
{
    [DllImport("VrLinkDIS")]
    public extern static IntPtr CreateDataQueryInteraction();

    [DllImport("VrLinkDIS")]
    public extern static void DeleteDataQueryInteraction(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static EntityId DQISenderId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDQISenderId(IntPtr interaction, EntityId senderId);

    [DllImport("VrLinkDIS")]
    public extern static EntityId DQIReceiverId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDQIReceiverId(IntPtr interaction, EntityId recieverId);

    [DllImport("VrLinkDIS")]
    public extern static ulong DQIRequestId(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDQIRequestId(IntPtr interaction, ulong requestId);

    [DllImport("VrLinkDIS")]
    public extern static void DQIInitDatumIds(IntPtr interaction, ulong numFixedFields, ulong numVarFields);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionNumFixedFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionFixedDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionSetFixedDatumId(IntPtr interaction, int index, int id);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionNumVarFields(IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionVarDatumId(IntPtr interaction, int index);

    [DllImport("VrLinkDIS")]
    public extern static int DataQueryInteractionSetVarDatumId(IntPtr interaction, int index, int id);

    [DllImport("VrLinkDIS")]
    public extern static void SendDQI(IntPtr exConn, IntPtr interaction);

    [DllImport("VrLinkDIS")]
    public extern static void SetDQIReceivedCallback(IntPtr exConnPtr, NetSimAgent.DQIReceivedCallback dIReceivedCallback);

}
