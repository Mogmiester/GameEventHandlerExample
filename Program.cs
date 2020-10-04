using System;
using System.Collections.Generic;
using Abilities;

namespace Game2
{
    public class Program
    {
        public HashSet<Unit> units = new HashSet<Unit>();
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.Game();
        }

        public void Game()
        {
            GameEventHandler.UnitDied += HandleUnitDeath;
            int newAvengers = 0;
            var im = new HashSet<DamageType> { DamageType.Fire };
            Unit originalAvenger = Unit.CreateUnit(this, "Bob the OG Avenger", 10, null, 2, new HashSet<Ability> { new Abilities.Avenger(2) });
            var defendingUnit = Unit.CreateUnit(this, "Example", 10, im, 5, new HashSet<Ability> { new Abilities.DeathAnouncer() });
            Console.WriteLine("Press 'x' to do fire damage, 'y' to do energy damage, 'z' to do dark damage");
            foreach (Unit u in units)
            {
                Console.WriteLine(u);
            }
            char keyCode = 'a';
            while (keyCode != 'q')
            {
                keyCode = Console.ReadKey(true).KeyChar;
                switch (keyCode)
                {
                    case 'x':
                        defendingUnit.DealDamage(DamageType.Fire, 1);
                        break;
                    case 'y':
                        defendingUnit.DealDamage(DamageType.Energy, 1);
                        break;
                    case 'z':
                        defendingUnit.DealDamage(DamageType.Darkness, 1);
                        break;
                    case 'b':
                        newAvengers++;
                        Unit u = Unit.CreateUnit(this, $"Dave {newAvengers} the Avenger", 10, null, 1, new HashSet<Ability> { new Abilities.Avenger(1) });
                        break;
                    case 'c':
                        var shielder = Unit.CreateUnit(this, "Shielder", 2, null, 0, new HashSet<Ability> { new Abilities.Shielder() });
                        break;
                    default:
                        break;
                }
                foreach (Unit u in units)
                {
                    Console.WriteLine(u);
                }
                Console.WriteLine();
            }
        }

        public void HandleUnitDeath(object sender, OnUnitDiedEventArgs e)
        {
            e.DeadUnit.Abilities.Clear();
            units.Remove(e.DeadUnit);
        }

    }

    public class Unit
    {
        public readonly HashSet<DamageType> Immunities;
        public HashSet<Abilities.Ability> Abilities;

        public static Unit CreateUnit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0, HashSet<Ability> Abilities = null)
        {
            Unit newUnit = new Unit(owner, name, health, immunities, weaponStrength);
            GameEventHandler.SpecificUnitDied.Add(newUnit, null);
            GameEventHandler.OnUnitCreated(newUnit);
            foreach (Ability a in Abilities ?? new HashSet<Ability>())
            {
                a.Apply(newUnit);
            }
            return newUnit;
        }

        private Unit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0)
        {
            Abilities = new HashSet<Ability>();
            Name = name;
            Health = health;
            Immunities = immunities ?? new HashSet<DamageType>();
            WeaponStrength = weaponStrength;
            Owner = owner;
            owner.units.Add(this);
        }

        public int Health { get; protected set; }
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
                int previousHealth = Health;
                GameEventHandler.OnUnitRecievedDamage(this, damage, damageWasRedirected);
                foreach (var (type, amt) in damage)
                {
                    if (!Immunities.Contains(type))
                    {
                        Health -= amt;
                    }
                    else
                    {
                        damage.Remove(type);
                    }
                }
                if (previousHealth != Health)
                {
                    GameEventHandler.OnUnitAppliedDamage(this, damage, damageWasRedirected);
                }
                if (Health > 0) return;
                Health = 0;
                GameEventHandler.OnUnitDied(this);
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