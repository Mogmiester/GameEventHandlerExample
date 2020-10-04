using System;
using System.Collections.Generic;

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
            Unit originalAvenger = CreateAvenger(this, "Bob the Avenger", 10, null, 2);
            foreach (Abilities.Ability a in originalAvenger.Abilities)
            {
                if (a.Name == "Avenger")
                {
                    Abilities.Avenger avenger = (Abilities.Avenger)a;
                    avenger.WeaponStrengthGain = 2;
                }
            }
            var defendingUnit = CreateUnit(this, "Example", 10, im);
            new Abilities.DeathAnouncer(defendingUnit);
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
                        CreateAvenger(this, $"Dave {newAvengers} the Avenger", 10, null, 1);
                        break;
                    case 'c':
                        var shielder = CreateUnit(this, "Shielder", 2, null, 0);
                        new Abilities.Shielder(shielder);
                        break;
                    default:
                        break;
                }
                foreach (Unit u in units)
                {
                    Console.WriteLine(u);
                }
            }
        }

        public Unit CreateUnit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0)
        {
            Unit newUnit = new Unit(owner, name, health, immunities, weaponStrength);
            GameEventHandler.OnUnitCreated(newUnit);
            units.Add(newUnit);
            return newUnit;
        }

        public Unit CreateAvenger(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0)
        {
            Unit newUnit = CreateUnit(owner, name, health, immunities, weaponStrength);
            new Abilities.Avenger(newUnit, 1);
            return newUnit;
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


        public Unit(Program owner, string name, int health, HashSet<DamageType> immunities = null, int weaponStrength = 0, HashSet<Abilities.Ability> abilities = null)
        {
            immunities ??= new HashSet<DamageType>();
            Name = name;
            Health = health;
            Immunities = immunities;
            WeaponStrength = weaponStrength;
            Owner = owner;
            Abilities = abilities ?? new HashSet<Abilities.Ability>();
        }

        public int Health { get; protected set; }
        public int WeaponStrength { get; protected set; }
        public string Name { get; }
        public Program Owner { get; private set; }
        public HashSet<Abilities.Ability> Abilities;

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