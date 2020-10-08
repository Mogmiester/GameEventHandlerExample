using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameEventHandlerExample;

namespace Abilities
{
    public abstract class Ability
    {
        public string Name = "Ability Name";
        public Unit UnitWithAbility { get; protected set; }
        public Ability(string name)
        {
            Name = name;
        }
        public virtual void Apply(Unit unitWithAbility)
        {
            UnitWithAbility = unitWithAbility;
            UnitWithAbility.AbilityList.Add(this);
            GameEventHandler.SpecificUnitDied[UnitWithAbility] += UnitWithAbilityDied;
        }
        public abstract void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e);
    }

    public abstract class AbilityActingOnAppliedDamage : Ability
    {
        protected Action<object, OnUnitAppliedDamageEventArgs> ActionActingOnAppliedDamage;
        public AbilityActingOnAppliedDamage(string name) : base(name)
        {
            GameEventHandler.UnitAppliedDamage += CallAction;
        }
        private void CallAction(object sender, OnUnitAppliedDamageEventArgs e)
        {
            if (ActionActingOnAppliedDamage is null) return;
            ActionActingOnAppliedDamage(sender, e);
        }

        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            GameEventHandler.UnitAppliedDamage -= CallAction;
            GameEventHandler.SpecificUnitDied[UnitWithAbility] -= UnitWithAbilityDied;
            UnitWithAbility = null;
        }
    }

    public abstract class AbilityActingOnRecievedDamage : Ability
    {
        protected Action<object, OnUnitRecievedDamageEventArgs> ActionActingOnAppliedDamage;
        public AbilityActingOnRecievedDamage(string name) : base(name)
        {
            GameEventHandler.UnitRecievedDamage += CallAction;
        }
        private void CallAction(object sender, OnUnitRecievedDamageEventArgs e)
        {
            if (ActionActingOnAppliedDamage is null) return;
            ActionActingOnAppliedDamage(sender, e);
        }

        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            GameEventHandler.UnitRecievedDamage -= CallAction;
            GameEventHandler.SpecificUnitDied[UnitWithAbility] -= UnitWithAbilityDied;
            UnitWithAbility = null;
        }
    }

    public class Avenger : AbilityActingOnAppliedDamage
    {
        public int WeaponStrengthGain;
        public Avenger(int weaponStrengthGain) : base("Avenger")
        {
            WeaponStrengthGain = weaponStrengthGain;
        }

        public override void Apply(Unit unitWithAbility)
        {
            ActionActingOnAppliedDamage = GainStrengthWhenAllyRecievesDamage;
            base.Apply(unitWithAbility);
        }

        public void GainStrengthWhenAllyRecievesDamage(object sender, OnUnitAppliedDamageEventArgs e)
        {
            if (!e.DamagedUnit.Equals(UnitWithAbility))
            {
                UnitWithAbility.AddWeaponStrength(WeaponStrengthGain);
            }
        }
    }
    public class DeathAnouncer : Ability
    {
        public DeathAnouncer() : base("Death Anouncer") { }

        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            Console.WriteLine("Important Unit Died!");
            GameEventHandler.SpecificUnitDied[UnitWithAbility] -= UnitWithAbilityDied;
            UnitWithAbility = null;
        }

    }

    public class Shielder : AbilityActingOnRecievedDamage
    {
        public Shielder() : base("Shielder")
        {
        }

        public override void Apply(Unit unitWithAbility)
        {
            ActionActingOnAppliedDamage = ShieldAllyFromEnergyDamage;
            base.Apply(unitWithAbility);
        }

        public void ShieldAllyFromEnergyDamage(object sender, OnUnitRecievedDamageEventArgs e)
        {
            if (!e.DamageWasRedirected && e.DamageDictionary.ContainsKey(DamageType.Energy) && !e.DamagedUnit.Equals(UnitWithAbility))
            {
                UnitWithAbility.DealDamage(DamageType.Energy, e.DamageDictionary[DamageType.Energy], true);
                e.DamageDictionary.Remove(DamageType.Energy);
            }
        }
    }
}
