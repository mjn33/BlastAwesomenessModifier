using System;

namespace BlastAwesomenessModifier {

public class BAMDebugPartModule : PartModule {
    [KSPField(guiActive = true, guiName = "BAM Power", guiFormat = "F5")]
    public float mBAMPower;

    //
    // Methods
    //
    [KSPEvent(name = "Explode", guiActive = true)]
    public void Explode() {
        base.part.explode();
    }

    internal void UpdatePower() {
        this.mBAMPower = base.part.explosionPotential;
    }
}

} // namespace BlastAwesomenessModifier
