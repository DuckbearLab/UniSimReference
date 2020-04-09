using UnityEngine;
using System.Collections;
using NetStructs;
using System;

namespace CppStructs
{

    public struct EntityState
    {
        public IntPtr reflectedEntityPtr;

        public EntityId entityId;
        public EntityType entityType;

        public double posX;
        public double posY;
        public double posZ;

        public double velX;
        public double velY;
        public double velZ;

        public double rotX;
        public double rotY;
        public double rotZ;

        public LifeformState lifeformState;
        public DamageState DamageState;
        public ForceType forceType;
        public WeaponState primaryWeaponState;
    }

    public struct EntityStateArtPart
    {
        public EntityId entityId;
        public uint partId;

        public double posX;
        public double posY;
        public double posZ;

        public double rotX;
        public double rotY;
        public double rotZ;
    }

    public struct XYZ
    {
        public double X;
        public double Y;
        public double Z;

        public XYZ(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XYZ(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XYZ(Vector3 Vector3)
        {
            X = Vector3.x;
            Y = Vector3.y;
            Z = Vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }

        public static implicit operator Vector3(XYZ xyz)
        {
            return xyz.ToVector3();
        }

        public static implicit operator XYZ(Vector3 Vector)
        {
            return new XYZ(Vector);
        }
    }

}
