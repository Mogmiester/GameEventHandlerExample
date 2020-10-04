using System;
using System.Collections.Generic;
using System.Text;
using Game2;

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
            UnitWithAbility.Abilities.Add(this);
            GameEventHandler.SpecificUnitDied[UnitWithAbility] += UnitWithAbilityDied;
        }
        public abstract void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e);
    }

    public class Avenger : Ability
    {
        public int WeaponStrengthGain;
        public Avenger(int weaponStrengthGain) : base("Avenger")
        {
            WeaponStrengthGain = weaponStrengthGain;
            GameEventHandler.UnitAppliedDamage += GainStrengthWhenAllyRecievesDamage;
        }

        public void GainStrengthWhenAllyRecievesDamage(object sender, OnUnitAppliedDamageEventArgs e)
        {
            if (!e.DamagedUnit.Equals(UnitWithAbility))
            {
                UnitWithAbility.AddWeaponStrength(WeaponStrengthGain);
            }
        }
        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            GameEventHandler.UnitAppliedDamage -= GainStrengthWhenAllyRecievesDamage;
            UnitWithAbility = null;
        }

    }
    public class DeathAnouncer : Ability
    {
        public DeathAnouncer() : base("Death Anouncer") { }

        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            Console.WriteLine("Important Unit Died!");
            UnitWithAbility = null;
        }

    }

    public class Shielder : Ability
    {
        public Shielder() : base("Shielder")
        {
            GameEventHandler.UnitRecievedDamage += ShieldAllyFromEnergyDamage;
        }

        public void ShieldAllyFromEnergyDamage(object sender, OnUnitRecievedDamageEventArgs e)
        {
            if (!e.DamageWasRedirected && e.DamageDictionary.ContainsKey(DamageType.Energy) && !e.DamagedUnit.Equals(UnitWithAbility))
            {
                UnitWithAbility.DealDamage(DamageType.Energy, e.DamageDictionary[DamageType.Energy], true);
                e.DamageDictionary.Remove(DamageType.Energy);
            }
        }
        public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
        {
            GameEventHandler.UnitRecievedDamage -= ShieldAllyFromEnergyDamage;
            UnitWithAbility = null;
        }
    }
}
