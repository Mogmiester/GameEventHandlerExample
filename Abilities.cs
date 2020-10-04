using System;
using System.Collections.Generic;
using System.Text;

namespace Game2
{
    public class Abilities
    {
        public abstract class Ability
        {
            public string Name = "Ability Name";
            public Unit UnitWithAbility { get; protected set; }
            public Ability(string name, Unit unitWithAbility)
            {
                Name = name;
                UnitWithAbility = unitWithAbility;
                UnitWithAbility.Abilities.Add(this);
                GameEventHandler.UnitDied += UnitWithAbilityDied;
            }

            public abstract void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e);

        }

        public class Avenger : Ability
        {
            public int WeaponStrengthGain;
            public Avenger(Unit unitWithAbility, int weaponStrengthGain) : base("Avenger", unitWithAbility)
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
                if (e.DeadUnit == UnitWithAbility)
                {
                    GameEventHandler.UnitAppliedDamage -= GainStrengthWhenAllyRecievesDamage;
                    UnitWithAbility = null;
                }
            }

        }
        public class DeathAnouncer : Ability
        {
            public DeathAnouncer(Unit unitWithAbility) : base("Death Anouncer", unitWithAbility)
            {
            }

            public override void UnitWithAbilityDied(object sender, OnUnitDiedEventArgs e)
            {
                if (e.DeadUnit == UnitWithAbility)
                {
                    Console.WriteLine("Important Unit Died!");
                    UnitWithAbility = null;
                }
            }

        }

        public class Shielder : Ability
        {
            public Shielder(Unit unitWithAbility) : base("Shielder", unitWithAbility)
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
                if (e.DeadUnit == UnitWithAbility)
                {
                    GameEventHandler.UnitRecievedDamage -= ShieldAllyFromEnergyDamage;
                    UnitWithAbility = null;
                }
            }
        }
    }
}
