using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    
    public LivingStatus TakeDamage(float amount, bool isPhysical);

}

