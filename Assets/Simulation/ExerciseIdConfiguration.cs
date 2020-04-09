using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
 * ExerciseIdConfiguration -
 * DESCRIPTION -
 * =================================================================================== */
namespace Simulation
{
    public class ExerciseIdConfiguration : ConfigurationScript<ExerciseIdConfiguration.Configuration>
    {
        [Serializable] public struct Configuration { public int ExerciseId; }

        public ExerciseConnection ExerciseConnection;

        protected override void ApplyConfiguration(bool GotConfiguration)
        {

            string arg = string.Empty;
            if (PutArg("ExerciseId", ref arg))
                ExerciseConnection.exerciseId = int.Parse(arg);
            else
                TryApplyTo(Config.ExerciseId, ref ExerciseConnection.exerciseId, "ExerciseId");
        }

#if UNITY_EDITOR
        void Reset()
        {
            ExerciseConnection = FindObjectOfType<ExerciseConnection>();
        }
#endif
    }
}