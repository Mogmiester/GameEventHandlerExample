using System;
using System.Collections.Generic;
using System.Text;
using Abilities;

namespace GameEventHandlerExample
{
    public class Unit
    {
        public readonly HashSet<DamageType> Immunities;
        public HashSet<Ability> AbilityList;

        public static Unit CreateUnit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0, HashSet<Ability> abilities = null)
        {
            Unit newUnit = new Unit(owner, name, health, immunities, weaponStrength);
            GameEventHandler.SpecificUnitDied.Add(newUnit, null);
            newUnit.AbilityList = abilities ?? new HashSet<Ability>();
            foreach (Ability a in newUnit.AbilityList)
            {
                a.Apply(newUnit);
            }
            GameEventHandler.OnUnitCreated(newUnit);
            return newUnit;
        }

        private Unit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0)
        {
            UnitIsAlive = true;
            Name = name;
            Health = health;
            Immunities = immunities ?? new HashSet<DamageType>();
            WeaponStrength = weaponStrength;
            Owner = owner;
            owner.units.Add(this);
        }
        public bool UnitIsAlive;
        private int _health;
        public int Health
        {
            get => _health;

            protected set
            {
                _health -= value;
                if (_health < 0)
                {
                    _health = 0;
                    UnitIsAlive = false;
                    GameEventHandler.OnUnitDied(this);
                }
            }
        }
        public int WeaponStrength { get; protected set; }
        public string Name { get; }
        public Program Owner { get; private set; }

        public void DealDamage(DamageType damage, int amt, bool damageWasRedirected = false)
        {
            Dictionary<DamageType, int> damageDictionary = new Dictionary<DamageType, int>();
            damageDictionary.Add(damage, amt);
            DealDamage(damageDictionary, damageWasRedirected);
        }

        public void DealDamage(Dictionary<DamageType, int> damage, bool damageWasRedirected = false)
        {
            if (Health > 0)
            {
                int totalDamage = 0;
                GameEventHandler.OnUnitRecievedDamage(this, damage, damageWasRedirected);
                foreach (var (type, amt) in damage)
                {
                    if (!Immunities.Contains(type))
                    {
                        totalDamage += amt;
                    }
                    else
                    {
                        damage.Remove(type);
                    }
                }
                if (totalDamage > 0)
                {
                    Health -= totalDamage;
                    GameEventHandler.OnUnitAppliedDamage(this, damage, damageWasRedirected);
                }
            }
        }

        public void AddWeaponStrength(int extraStrength)
        {
            WeaponStrength += extraStrength;
        }

        public override string ToString()
        {
            return $"Unit {Name} : {WeaponStrength} Attack : {Health} HP";
        }
    }
}
