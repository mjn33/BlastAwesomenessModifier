using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlastAwesomenessModifier {

public class BAMPartModule : PartModule {
    public BAMPartModule() {
    }

    [KSPField(isPersistant = false, guiActive = false)]
    public float fixedValue = -1.0f;

    private BAMDebugPartModule mDebugModule;

    public void Start() {
        if (!BAMAddon.Instance.DebugEnabled) {
            return;
        }

        mDebugModule = part.FindModuleImplementing<BAMDebugPartModule>();
        if (mDebugModule == null) {
            mDebugModule = (BAMDebugPartModule)part.AddModule("BAMDebugPartModule");
        }
        if (mDebugModule != null) {
            mDebugModule.UpdatePower();
        }
    }

    public void FixedUpdate() {
        if (mDebugModule != null) {
            mDebugModule.UpdatePower();
        }

        if (fixedValue < 0) {
            part.explosionPotential = calcExplosionPotential();
        } else {
            part.explosionPotential = fixedValue;
        }
    }

    private float getFixedValue() {
        return Mathf.Clamp(BAMAddon.Instance.Base + fixedValue, 0, 50000);
    }

    private float calcExplosionPotential() {
        float sum = 0;

        foreach (KeyValuePair<string, PartResourceDefinition> def in BAMAddon.Instance.ResourceDefs) {
            PartResource resource = part.Resources.Get(def.Value.id);
            if (resource != null) {
                float modVal = BAMAddon.Instance.ResourceConfigs[def.Key];
                sum += modVal * (float)resource.amount;
            }
        }

        return Mathf.Clamp(BAMAddon.Instance.Base + sum, BAMAddon.Instance.Min, BAMAddon.Instance.Max);
    }
}

} // namespace BlastAwesomenessModifier

