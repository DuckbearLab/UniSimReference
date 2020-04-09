using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CppStructs;
using System;
using Newtonsoft.Json;

namespace NetStructs
{
    public enum LifeformState
    {
        NA,
        Standing,
        Walking,
        Running,
        Kneeling,
        Prone,
        Crawling,
        Swimming,
        Parachuting,
        Jumping,
        Sitting,
        Squatting,
        Crouching,
        Wading
    }

    public enum DamageState
    {
        None,
        Slight,
        Moderate,
        Destroyed
    }

    public enum WeaponState
    {
        NoWeapon,
        WeaponStowed,
        WeaponDeployed,
        WeaponInFirePosition,
        WeaponReloading
    };

    public enum ForceType
    {
        DtForceOther,
        DtForceFriendly,
        DtForceOpposing,
        DtForceNeutral
    }
	
    public enum AcknowledgeFlag
    {
        DtAckCreate = 1,
        DtAckRemove,
        DtAckStart,
        DtAckStop,
        DtTransferControl
    }

    public enum ActionType
    {
        ACTION_TYPE_NULL = 0,
        ACTION_TYPE_ADD_UPDATE = 1,
        ACTION_TYPE_REMOVE = 2
    }

    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    [Serializable]
    public struct EntityId : IEquatable<EntityId>, IUniqueKeyGenerator<EntityId>
    {
        public int site;
        public int host;
        public ushort entity;

        public EntityId(int site = 0, int host = 0, ushort entity = 0)
        {
            this.site = site;
            this.host = host;
            this.entity = entity;
        }

        public static EntityId NullId { get { return new EntityId() { site = 0, host = 0, entity = 0 }; } }

        public bool IsNullId { get { return site == 0 && host == 0 && entity == 0; } }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static EntityId FromString(string entityIdString)
        {
            var parts = entityIdString.Split(':');

            if (parts.Length != 3) throw new Exception("Invalid entityId string");

            return new EntityId() { site = int.Parse(parts[0]), host = int.Parse(parts[1]), entity = (ushort)int.Parse(parts[2]) };
        }

        public static bool TryFromString(string entityIdString, out EntityId Id, bool Log = true)
        {
            Id = new EntityId();
            var parts = entityIdString.Split(':');

            if (parts.Length != 3)
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            int site, host;
            ushort entity;
            if (!int.TryParse(parts[0], out site))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[1], out host))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!ushort.TryParse(parts[2], out entity))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            Id.site = site; Id.host = host; Id.entity = entity;
            return true;
        }

        public override bool Equals(object other)
        {
            if (!(other is EntityId))
                return false;

            return Equals((EntityId)other);
        }

        public bool Equals(EntityId other)
        {
            return (site == other.site && host == other.host && entity == other.entity);
        }

        public override string ToString()
        {
            return site + ":" + host + ":" + entity;
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return (left.site == right.site && left.host == right.host && left.entity == right.entity);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !(left == right);
        }

        public static explicit operator EntityId(string Id)
        {
            return FromString(Id);
        }

        public static explicit operator string (EntityId Id)
        {
            return Id.ToString();
        }

        public EntityId GenerateUniqueKey(HashSet<EntityId> presentKeys)
        {
            int i = 0;
            EntityId id;
            do
            {
                id = new EntityId(i, i, (ushort)i);
                i++;
            }
            while (presentKeys.Contains(id));
            return id;
        }
    }

    public struct EventID
    {
        public int site;
        public int host;
        public int eventNum;

        public static EventID NullId { get { return new EventID() { site = 0, host = 0, eventNum = 0 }; } }

        public bool IsNullId { get { return Equals(NullId); } }

        public static EventID FromString(string eventIdString)
        {
            var parts = eventIdString.Split(':');

            if (parts.Length != 3) throw new Exception("Invalid EventID string");

            return new EventID() { site = int.Parse(parts[0]), host = int.Parse(parts[1]), eventNum = int.Parse(parts[2]) };
        }

        public override bool Equals(object other)
        {
            if (!(other is EventID))
                return false;

            EventID item = (EventID)other;

            return (site == item.site && host == item.host && eventNum == item.eventNum);
        }

        public bool Equals(EventID other)
        {
            return (site == other.site && host == other.host && eventNum == other.eventNum);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return site + ":" + host + ":" + eventNum;
        }
    }

    [Serializable]
    public struct EntityType : IComparable<EntityType>, IUniqueKeyGenerator<EntityType>
    {
        public int entityKind;
        public int domain;
        public int country;
        public int category;
        public int subCategory;
        public int specific;
        public int extra;

        public static EntityType NullType { get { return new EntityType() { entityKind = 0, domain = 0, country = 0, category = 0, subCategory = 0, specific = 0, extra = 0 }; } }

        public EntityType(int entityKind, int domain, int country, int category, int subCategory, int specific, int extra)
        {
            this.entityKind = entityKind;
            this.domain = domain;
            this.country = country;
            this.category = category;
            this.subCategory = subCategory;
            this.specific = specific;
            this.extra = extra;
        }

        public bool IsNullType { get { return Equals(NullType); } }

        public static EntityType FromString(string entityTypeString)
        {
            var parts = entityTypeString.Split(':');

            if (parts.Length != 7) throw new Exception("Invalid EntityType string");

            return new EntityType()
            {
                entityKind = int.Parse(parts[0]),
                domain = int.Parse(parts[1]),
                country = int.Parse(parts[2]),
                category = int.Parse(parts[3]),
                subCategory = int.Parse(parts[4]),
                specific = int.Parse(parts[5]),
                extra = int.Parse(parts[6])
            };
        }

        public static bool TryFromString(string entityTypeString, out EntityType Type, bool Log = true)
        {
            Type = new EntityType();
            var parts = entityTypeString.Split(':');

            if (parts.Length != 7)
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            int entityKind, domain, country, category, subCategory, specific, extra;
            if (!int.TryParse(parts[0], out entityKind))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[1], out domain))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[2], out country))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[3], out category))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[4], out subCategory))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[5], out specific))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            if (!int.TryParse(parts[6], out extra))
            {
                if (Log) Debug.LogError("Invalid EntityId string, returning NullId.");
                return false;
            }
            Type = new EntityType()
            {
                entityKind = entityKind,
                domain = domain,
                country = country,
                category = category,
                subCategory = subCategory,
                specific = specific,
                extra = extra
            };
            return true;
        }

        public override bool Equals(object other)
        {
            if (!(other is EntityType))
                return false;

            EntityType item = (EntityType)other;

            return (entityKind == item.entityKind &&
                    domain == item.domain &&
                    country == item.country &&
                    category == item.category &&
                    subCategory == item.subCategory &&
                    specific == item.specific &&
                    extra == item.extra);
        }

        public bool Equals(EntityType other)
        {
            return (entityKind == other.entityKind &&
                    domain == other.domain &&
                    country == other.country &&
                    category == other.category &&
                    subCategory == other.subCategory &&
                    specific == other.specific &&
                    extra == other.extra);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return entityKind + ":" + domain + ":" + country + ":" + category + ":" + subCategory + ":" + specific + ":" + extra;
        }

        public bool MatchPattern(EntityType entityType)
        {
            if (!(this.entityKind == -1 || entityType.entityKind == -1 || this.entityKind == entityType.entityKind))
                return false;

            if (!(this.domain == -1 || entityType.domain == -1 || this.domain == entityType.domain))
                return false;

            if (!(this.country == -1 || entityType.country == -1 || this.country == entityType.country))
                return false;

            if (!(this.category == -1 || entityType.category == -1 || this.category == entityType.category))
                return false;

            if (!(this.subCategory == -1 || entityType.subCategory == -1 || this.subCategory == entityType.subCategory))
                return false;

            if (!(this.specific == -1 || entityType.specific == -1 || this.specific == entityType.specific))
                return false;

            if (!(this.extra == -1 || entityType.extra == -1 || this.extra == entityType.extra))
                return false;

            return true;
        }

        public bool isLessEntityType(EntityType entityType)
        {
            if (this.entityKind < entityType.entityKind)
            {
                return true;
            }
            if (this.domain < entityType.domain)
            {
                return true;
            }
            if (this.country < entityType.country)
            {
                return true;
            }
            if (this.category < entityType.category)
            {
                return true;
            }
            if (this.subCategory < entityType.subCategory)
            {
                return true;
            }
            if (this.specific < entityType.specific)
            {
                return true;
            }
            if (this.extra < entityType.extra)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compare this EntityType to another by order of fields (entityKind, domain, country, category, subCategory, specific, extra). 
        /// </summary>
        /// <param name="other">The other entity type to compare to. </param>
        /// <returns>
        /// Positive value if the currently checked field is larger than other's matching field, 
        /// Negative value if it is smaller, or zero if all fields are equal.
        /// </returns>
        public int CompareTo(EntityType other)
        {
            int cKind = entityKind.CompareTo(other.entityKind);
            if (0 == cKind)
            {
                int cDomain = domain.CompareTo(other.domain);
                if (0 == cDomain)
                {
                    int cCountry = country.CompareTo(other.country);
                    if (0 == cCountry)
                    {
                        int cCategory = category.CompareTo(other.category);
                        if (0 == cCategory)
                        {
                            int cSubCategory = subCategory.CompareTo(other.subCategory);
                            if (0 == cSubCategory)
                            {
                                int cSpecific = specific.CompareTo(other.specific);
                                if (0 == cSpecific)
                                {
                                    return extra.CompareTo(other.extra);
                                }
                                else return cSpecific;
                            }
                            else return cSubCategory;
                        }
                        else return cCategory;
                    }
                    else return cCountry;
                }
                else return cDomain;
            }
            else return cKind;
        }

        public static bool operator ==(EntityType left, EntityType right)
        {
            return (left.entityKind == right.entityKind &&
                    left.domain == right.domain &&
                    left.country == right.country &&
                    left.category == right.category &&
                    left.subCategory == right.subCategory &&
                    left.specific == right.specific &&
                    left.extra == right.extra);
        }

        public static bool operator !=(EntityType left, EntityType right)
        {
            return (left.entityKind     != right.entityKind     ||
                    left.domain         != right.domain         ||
                    left.country        != right.country        ||
                    left.category       != right.category       ||
                    left.subCategory    != right.subCategory    ||
                    left.specific       != right.specific       ||
                    left.extra          != right.extra);
        }

        public static explicit operator EntityType(string Id)
        {
            return FromString(Id);
        }

        public static explicit operator string (EntityType Id)
        {
            return Id.ToString();
        }

        public EntityType GenerateUniqueKey(HashSet<EntityType> presentKeys)
        {
            int i = 0;
            EntityType type;
            do
            {
                type = new EntityType(i, i, i, i, i, i, i);
                i++;
            } while (presentKeys.Contains(type));
            return type;
        }
    }

    public struct FireInteraction
    {
        public EntityId attacker;
        public EntityType munitionType;
        public EntityId target;
        public EntityId munition;
        public EventID eventId;
        public XYZ linVelocity;
        public XYZ location; //local pos
        public double range;

        public int quantity;
        public int rate;
    };

    public struct DetonationInteraction
    {
        public EntityId attacker;
        public EntityType munitionType;
        public EntityId target;
        public EntityId munition;
        public EventID eventId;
        public XYZ linVelocity;
        public XYZ worldLocation; //local pos
        public XYZ entityLocation; //local pos

        public int quantity;
        public int rate;
    };

    public struct CreateEntityInteraction
    {
        public EntityId senderId;
        public EntityId receiverId;
        public int requestId;
    };

    public struct RemoveEntityInteraction
    {
        public EntityId senderId;
        public EntityId receiverId;
        public int requestId;
    };

    public struct AcknowledgeInteraction
    {
        public EntityId senderId;
        public EntityId receiverId;
        public AcknowledgeFlag acknowledgeFlag;
        public int requestId;
    };

    namespace Converters
    {
        [JsonConverter(typeof(EntityId))]
        public class EntityIdSerializer : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(EntityId);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return EntityId.FromString(reader.ReadAsString());
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {

                writer.WriteValue(((EntityId)value).ToString());
            }
            public override bool CanRead { get { return true; } }
            public override bool CanWrite { get { return true; } }
        }

        [JsonConverter(typeof(EntityType))]
        public class EntityTypeSerializer : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(EntityType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return EntityType.FromString(reader.ReadAsString());
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {

                writer.WriteValue(((EntityType)value).ToString());
            }
            public override bool CanRead { get { return true; } }
            public override bool CanWrite { get { return true; } }
        }

    }

#if UNITY_EDITOR
    namespace PropertyDrawers
    {
        [UnityEditor.CustomPropertyDrawer(typeof(EntityId))]
        public class EntityIdPropertyDrawer : UnityEditor.PropertyDrawer
        {
            private const float TextWidth = 25;
            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                return 16;
            }

            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                UnityEditor.SerializedProperty site = property.FindPropertyRelative("site"),
                    host = property.FindPropertyRelative("host"),
                    entity = property.FindPropertyRelative("entity");
                float ShiftLeft = 6 + 8 * UnityEditor.EditorGUI.indentLevel;
                UnityEditor.EditorGUI.BeginProperty(position, label, property);
                float titleWidth = UnityEditor.EditorGUIUtility.labelWidth;
                Rect labelR = new Rect(position);
                labelR.width = titleWidth;
                UnityEditor.EditorGUI.LabelField(labelR, label);
                float indentationFix = 15 * UnityEditor.EditorGUI.indentLevel;
                float totalTextWidth = 2 * TextWidth - 4 * ShiftLeft;
                float partWidth = (position.width - titleWidth - totalTextWidth + indentationFix) / 3;
                Rect siteR = new Rect(labelR.xMax - indentationFix, position.y, partWidth, position.height),
                    colon1R = new Rect(siteR.xMax - ShiftLeft, position.y, TextWidth, position.height),
                    hostR = new Rect(colon1R.xMax - ShiftLeft, position.y, partWidth, position.height),
                    colon2R = new Rect(hostR.xMax - ShiftLeft, position.y, TextWidth, position.height),
                    entityR = new Rect(colon2R.xMax - ShiftLeft, position.y, partWidth, position.height);
                UnityEditor.EditorGUI.PropertyField(siteR, site, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(hostR, host, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(entityR, entity, GUIContent.none);
                GUIStyle s = new GUIStyle();
                s.alignment = TextAnchor.MiddleCenter;
                UnityEditor.EditorGUI.LabelField(colon1R, ":", s);
                UnityEditor.EditorGUI.LabelField(colon2R, ":", s);

                Rect propertyArea = Rect.MinMaxRect(siteR.xMin, siteR.yMin, entityR.xMax, entityR.yMax);
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.button == 1 && propertyArea.Contains(e.mousePosition))
                {
                    TextEditor te = new TextEditor();
                    UnityEditor.GenericMenu menu = new UnityEditor.GenericMenu();
                    string value = new System.Text.StringBuilder()
                            .Append(site.intValue).Append(":")
                            .Append(host.intValue).Append(":")
                            .Append(entity.intValue).ToString();
                    GUIContent content = new GUIContent("Copy " + value, value);
                    menu.AddItem(content, false, () =>
                    {
                        te.text = value;
                        te.SelectAll();
                        te.Copy();
                    });
                    te.text = string.Empty;
                    if (te.CanPaste())
                    {
                        te.Paste();
                        EntityId id;
                        if (EntityId.TryFromString(te.text, out id, false))
                        {
                            menu.AddItem(new GUIContent("Paste " + te.text, te.text), false, () =>
                            {
                                site.intValue = id.site;
                                host.intValue = id.host;
                                entity.intValue = id.entity;
                                property.serializedObject.ApplyModifiedProperties();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Paste as string"));
                        }
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste as string"));
                    }
                    menu.ShowAsContext();
                }
                UnityEditor.EditorGUI.EndProperty();
            }
        }


        [UnityEditor.CustomPropertyDrawer(typeof(EntityType))]
        public class EntityTypePropertyDrawer : UnityEditor.PropertyDrawer
        {
            private const float TextWidth = 25;
            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                return 35;
            }

            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                UnityEditor.SerializedProperty
                    entityKind = property.FindPropertyRelative("entityKind"),
                    domain = property.FindPropertyRelative("domain"),
                    country = property.FindPropertyRelative("country"),
                    category = property.FindPropertyRelative("category"),
                    subCategory = property.FindPropertyRelative("subCategory"),
                    specific = property.FindPropertyRelative("specific"),
                    extra = property.FindPropertyRelative("extra");
                float ShiftLeft = 8 + 7 * UnityEditor.EditorGUI.indentLevel;
                UnityEditor.EditorGUI.BeginProperty(position, label, property);
                float titleWidth = UnityEditor.EditorGUIUtility.labelWidth;
                Rect labelR = new Rect(position);
                labelR.width = titleWidth;
                UnityEditor.EditorGUI.LabelField(labelR, label);
                float indentationFix = 15 * UnityEditor.EditorGUI.indentLevel;
                float totalTextWidth = 4 * TextWidth - 6 * ShiftLeft;
                float partWidth = 4 + (position.width - titleWidth - totalTextWidth + indentationFix) / 4;
                Rect kindR = new Rect(labelR.xMax - indentationFix, position.y, partWidth, 16),
                    colon1R = new Rect(kindR.x + partWidth - ShiftLeft, position.y, TextWidth, 16),
                    domainR = new Rect(colon1R.x + colon1R.width - ShiftLeft, position.y, partWidth, 16),
                    colon2R = new Rect(domainR.x + partWidth - ShiftLeft, position.y, TextWidth, 16),
                    countryR = new Rect(colon2R.x + colon2R.width - ShiftLeft, position.y, partWidth, 16),
                    colon3R = new Rect(countryR.x + partWidth - ShiftLeft, position.y, TextWidth, 16),
                    categoryR = new Rect(colon3R.x + colon3R.width - ShiftLeft, position.y, partWidth, 16),
                    colon4R = new Rect(categoryR.x + partWidth - ShiftLeft, position.y, TextWidth, 16),
                    subCategoryR = new Rect(kindR.x, position.yMax - 16, partWidth, 16),
                    colon5R = new Rect(colon1R.x, subCategoryR.y, TextWidth, 16),
                    specificR = new Rect(domainR.x, colon5R.y, partWidth, 16),
                    colon6R = new Rect(colon2R.x, specificR.y, TextWidth, 16),
                    extraR = new Rect(countryR.x, colon6R.y, partWidth, 16);
                GUIStyle s = new GUIStyle();
                s.alignment = TextAnchor.MiddleCenter;
                UnityEditor.EditorGUI.PropertyField(kindR, entityKind, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon1R, ":", s);
                UnityEditor.EditorGUI.PropertyField(domainR, domain, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon2R, ":", s);
                UnityEditor.EditorGUI.PropertyField(countryR, country, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon3R, ":", s);
                UnityEditor.EditorGUI.PropertyField(categoryR, category, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon4R, ":", s);
                UnityEditor.EditorGUI.PropertyField(subCategoryR, subCategory, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon5R, ":", s);
                UnityEditor.EditorGUI.PropertyField(specificR, specific, GUIContent.none);
                UnityEditor.EditorGUI.LabelField(colon6R, ":", s);
                UnityEditor.EditorGUI.PropertyField(extraR, extra, GUIContent.none);
                UnityEditor.EditorGUI.EndProperty();

                Rect propertyArea = Rect.MinMaxRect(kindR.xMin, kindR.yMin, categoryR.xMax + TextWidth, extraR.yMax);
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.button == 1 && propertyArea.Contains(e.mousePosition))
                {
                    UnityEditor.GenericMenu menu = new UnityEditor.GenericMenu();
                    TextEditor te = new TextEditor();
                    string value = new System.Text.StringBuilder()
                            .Append(entityKind.intValue).Append(":")
                            .Append(domain.intValue).Append(":")
                            .Append(country.intValue).Append(":")
                            .Append(category.intValue).Append(":")
                            .Append(subCategory.intValue).Append(":")
                            .Append(specific.intValue).Append(":")
                            .Append(extra.intValue).ToString();
                    menu.AddItem(new GUIContent("Copy " + value, value), false, () =>
                    {
                        te.text = value;
                        te.SelectAll();
                        te.Copy();
                    });
                    te.text = string.Empty;
                    if (te.CanPaste())
                    {
                        te.Paste();
                        EntityType type;
                        if (EntityType.TryFromString(te.text, out type, false))
                        {
                            menu.AddItem(new GUIContent("Paste " + te.text, te.text), false, () =>
                            {
                                entityKind.intValue = type.entityKind;
                                domain.intValue = type.domain;
                                country.intValue = type.country;
                                category.intValue = type.category;
                                subCategory.intValue = type.subCategory;
                                specific.intValue = type.specific;
                                extra.intValue = type.extra;
                                property.serializedObject.ApplyModifiedProperties();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Paste as string", te.text));
                        }
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste as string"));
                    }
                    menu.ShowAsContext();
                }
            }
        }
    }
#endif
}


