using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class SingletonParticleSystem<T> : MonoBehaviour where T : MonoBehaviour
{
    private static SingletonParticleSystem<T> m_instance = null;

    protected ParticleSystem m_particleSystem = null;
    protected ParticleSystem.EmitParams m_emitParams;

    private void Awake()
    {
        m_instance = this;

        TryGetComponent(out m_particleSystem);
        m_emitParams = new ParticleSystem.EmitParams();
        m_emitParams.applyShapeToPosition = true;

        modifyEmitParameters();
    }

    public static void PlayAtPosition(Vector3 position, Vector3 normal)
    {
        m_instance.m_emitParams.position = position;

        ParticleSystem.ShapeModule _shapeModule = m_instance.m_particleSystem.shape;
        _shapeModule.rotation = Quaternion.LookRotation(normal).eulerAngles;

        int _particleCount = m_instance.applyNewEmitParameters();

        m_instance.m_particleSystem.Emit(m_instance.m_emitParams, _particleCount);
    }

    protected virtual void modifyEmitParameters() { }
    protected abstract int applyNewEmitParameters();
}
