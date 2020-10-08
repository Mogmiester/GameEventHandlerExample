using System;
using System.Collections.Generic;
using System.Text;

namespace GameEventHandlerExample
{
    public static class GameEventHandler
    {
        public static EventHandler<OnUnitCreatedEventArgs> UnitCreated;
        public static EventHandler<OnUnitRecievedDamageEventArgs> UnitRecievedDamage;
        public static EventHandler<OnUnitAppliedDamageEventArgs> UnitAppliedDamage;
        public static EventHandler<OnUnitDiedEventArgs> UnitDied;
        public static Dictionary<Unit, EventHandler<OnUnitDiedEventArgs>> SpecificUnitDied = new Dictionary<Unit, EventHandler<OnUnitDiedEventArgs>>();

        public static void OnUnitCreated(Unit createdUnit)
        {
            UnitCreated?.Invoke(null, new OnUnitCreatedEventArgs(createdUnit));
        }

        public static void OnUnitRecievedDamage(Unit damagedUnit, Dictionary<DamageType, int> damageDictionary, bool damageWasRedirected = false)
        {
            UnitRecievedDamage?.Invoke(null, new OnUnitRecievedDamageEventArgs(damagedUnit, damageDictionary, damageWasRedirected));
        }

        public static void OnUnitAppliedDamage(Unit damagedUnit, Dictionary<DamageType, int> damageDictionary, bool damageWasRedirected = false)
        {
            UnitAppliedDamage?.Invoke(null, new OnUnitAppliedDamageEventArgs(damagedUnit, damageDictionary, damageWasRedirected));
        }
        public static void OnUnitDied(Unit diedUnit)
        {
            SpecificUnitDied[diedUnit]?.Invoke(null, new OnUnitDiedEventArgs(diedUnit));
            UnitDied?.Invoke(null, new OnUnitDiedEventArgs(diedUnit));
        }
    }

    public class OnUnitCreatedEventArgs : EventArgs
    {
        public Unit CreatedUnit;

        public OnUnitCreatedEventArgs(Unit createdUnit)
        {
            CreatedUnit = createdUnit;
        }
    }
    public class OnUnitRecievedDamageEventArgs : EventArgs
    {
        public Unit DamagedUnit;
        public Dictionary<DamageType, int> DamageDictionary;
        public bool DamageWasRedirected;

        public OnUnitRecievedDamageEventArgs(Unit damagedUnit, Dictionary<DamageType, int> damageDictionary, bool damageWasRedirected = false)
        {
            DamagedUnit = damagedUnit;
            DamageDictionary = damageDictionary;
            DamageWasRedirected = damageWasRedirected;
        }
    }

    public class OnUnitAppliedDamageEventArgs : EventArgs
    {
        public Unit DamagedUnit;
        public Dictionary<DamageType, int> DamageDictionary;
        public bool DamageWasRedirected;

        public OnUnitAppliedDamageEventArgs(Unit damagedUnit, Dictionary<DamageType, int> damageDictionary, bool damageWasRedirected = false)
        {
            DamagedUnit = damagedUnit;
            DamageDictionary = damageDictionary;
            DamageWasRedirected = damageWasRedirected;
        }
    }

    public class OnUnitDiedEventArgs : EventArgs
    {
        public Unit DeadUnit;

        public OnUnitDiedEventArgs(Unit deadUnit)
        {
            DeadUnit = deadUnit;
        }
    }
}
