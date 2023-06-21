using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMonster Data", menuName = "Scriptable Object/BaseMonster Data", order = 1)]
public class BaseMonsterData : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("���� ����")]
    [SerializeField] List<AgressiveMonsterData> _agreesiveMonsterData = new List<AgressiveMonsterData>();
    [SerializeField] List<MeleeMonsterData> _meleeMonsterData = new List<MeleeMonsterData>();
    [SerializeField] List<RangeMonsterData> _rangeMonsterData = new List<RangeMonsterData>();
    [SerializeField] int _maxHp;

    [NonSerialized] public List<AgressiveMonsterData> AgressiveMonsterData;
    [NonSerialized] public List<MeleeMonsterData> MeleeMonsterData;
    [NonSerialized] public List<RangeMonsterData> RangeMonsterData;
    [NonSerialized] public int MaxHp;

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        AgressiveMonsterData = _agreesiveMonsterData;
        MeleeMonsterData = _meleeMonsterData;
        RangeMonsterData = _rangeMonsterData;
        MaxHp = _maxHp;
    }
}