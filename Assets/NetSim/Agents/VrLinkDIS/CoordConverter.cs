using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using CppStructs;

public class CoordConverter
{
    public static Vector3 LocalToUtm(Vector3 local)
    {
        return NetSimAgent.Instance.localToUtm(local.x, local.y, local.z);
    }

    public static Vector3 LocalToUtm(float x,float y,float z)
    {
        return NetSimAgent.Instance.localToUtm(x, y, z);
    }

    public static Vector3 UtmToLocal(Vector3 local)
    {
        return NetSimAgent.Instance.utmToLocal(local.x, local.y, local.z);
    }

    public static Vector3 UtmToLocal(float x, float y, float z)
    {
        return NetSimAgent.Instance.utmToLocal(x, y, z);
    }

    public static Vector3 LocalToGeoc(Vector3 local)
    {
        return NetSimAgent.Instance.localToGeoc(local.x, local.y, local.z);
    }

    public static Vector3 LocalToGeoc(float x, float y, float z)
    {
        return NetSimAgent.Instance.localToGeoc(x, y, z);
    }

    public static Vector3 GeocToLocal(Vector3 Geoc)
    {
        return NetSimAgent.Instance.geocToLocal(Geoc.x, Geoc.y, Geoc.z);
    }

    public static Vector3 GeocToLocal(XYZ Geoc)
    {
        return NetSimAgent.Instance.geocToLocal(Geoc.X, Geoc.Y, Geoc.Z);
    }

    public static Vector3 GeocToLocal(float x, float y, float z)
    {
        return NetSimAgent.Instance.geocToLocal(x, y, z);
    }

    public static Vector3 LocalToGeod(Vector3 local)
    {
        return NetSimAgent.Instance.localToGeod(local.x, local.y, local.z);
    }

    public static Vector3 LocalToGeod(float x, float y, float z)
    {
        return NetSimAgent.Instance.localToGeod(x, y, z);
    }

    public static Vector3 GeodToLocal(Vector3 local)
    {
        return NetSimAgent.Instance.geodToLocal(local.x, local.y, local.z);
    }

    public static Vector3 GeodToLocal(XYZ local)
    {
        return NetSimAgent.Instance.geodToLocal(local.X, local.Y, local.Z);
    }

    public static Vector3 GeodToLocal(float x, float y, float z)
    {
        return NetSimAgent.Instance.geodToLocal(x, y, z);
    }

    public static Vector3 UtmWgs84ToEd50(Vector3 wgs84)
    {
        return NetSimAgent.Instance.utmWgs84ToEd50(wgs84.x, wgs84.y, wgs84.z);
    }

    public static Vector3 UtmWgs84ToEd50(float x, float y, float z)
    {
        return NetSimAgent.Instance.utmWgs84ToEd50(x, y, z);
    }

    public static Vector3 UtmEd50ToWgs84(Vector3 ed50)
    {
        return NetSimAgent.Instance.utmEd50ToWgs84(ed50.x, ed50.y, ed50.z);
    }

    public static Vector3 UtmEd50ToWgs84(float x, float y, float z)
    {
        return NetSimAgent.Instance.utmEd50ToWgs84(x, y, z);
    }

    public static Vector3 LocalToUtmEd50(Vector3 local)
    {
        Vector3 wgs84 = LocalToUtm(local);
        return NetSimAgent.Instance.utmWgs84ToEd50(wgs84.x, wgs84.y, wgs84.z);
    }

    public static Vector3 LocalToUtmEd50(float x, float y, float z)
    {
        Vector3 wgs84 = LocalToUtm(x, y, z);
        return NetSimAgent.Instance.utmWgs84ToEd50(wgs84.x, wgs84.y, wgs84.z);
    }

    public static Vector3 UtmEd50ToLocal(Vector3 ed50)
    {
        Vector3 wgs84 = UtmEd50ToWgs84(ed50);
        return NetSimAgent.Instance.utmToLocal(wgs84.x, wgs84.y, wgs84.z);
    }

    public static Vector3 UtmEd50ToLocal(float x, float y, float z)
    {
        Vector3 wgs84 = UtmEd50ToWgs84(x, y, z);
        return NetSimAgent.Instance.utmToLocal(wgs84.x, wgs84.y, wgs84.z);
    }
}
